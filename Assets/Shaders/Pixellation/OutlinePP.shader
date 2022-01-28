Shader "FullScreen/OutlinePP"
{
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _Thickness("OutlineThickness", float) = 0.1
        _DpthStr("DepthStrength", float) = 1.0
        _DpthThickness("DepthThickness", float) = 1.0
        _DpthThreshhold("DepthThreshhold", float) = 1.0
        _ColStr("ColorStrength", float) = 1.0
        _ColThickness("ColorThickness", float) = 1.0
        _ColThreshhold("ColorThreshhold", float) = 1.0

    }
    HLSLINCLUDE

    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
    
    //The PositionInputs struct allow you to retrieve a lot of useful information for your fullScreenShader:
    // struct PositionInputs
    // {
    //     float3 positionWS;  // World space position (could be camera-relative)
    //     float2 positionNDC; // Normalized screen coordinates within the viewport    : [0, 1) (with the half-pixel offset)
    //     uint2  positionSS;  // Screen space pixel coordinates                       : [0, NumPixels)
    //     uint2  tileCoord;   // Screen tile coordinates                              : [0, NumTiles)
    //     float  deviceDepth; // Depth from the depth buffer                          : [0, 1] (typically reversed)
    //     float  linearDepth; // View space Z coordinate                              : [Near, Far]
    // };

    // To sample custom buffers, you have access to these functions:
    // But be careful, on most platforms you can't sample to the bound color buffer. It means that you
    // can't use the SampleCustomColor when the pass color buffer is set to custom (and same for camera the buffer).
    // float4 SampleCustomColor(float2 uv);
    // float4 LoadCustomColor(uint2 pixelCoords);
    // float LoadCustomDepth(uint2 pixelCoords);
    // float SampleCustomDepth(float2 uv);

    // There are also a lot of utility function you can use inside Common.hlsl and Color.hlsl,
    // you can check them out in the source code of the core SRP package.

    // These are points to sample relative to the starting point
    static float2 sobelSamplePoints[9] = {
        float2(-1, 1), float2(0, 1), float2(1, 1),
        float2(-1, 0), float2(0, 0), float2(1, 0),
        float2(-1, -1), float2(0, -1), float2(1, -1),
    };

    // Weights for the x component
    static float sobelXMatrix[9] = {
        1, 0, -1,
        2, 0, -2,
        1, 0, -1
    };

    // Weights for the y component
    static float sobelYMatrix[9] = {
        1, 2, 1,
        0, 0, 0,
        -1, -2, -1
    };

    float4 _Color;
    float _Thickness;
    float _DpthStr;
    float _DpthThickness;
    float _DpthThreshhold;
    float _ColStr;
    float _ColThickness;
    float _ColThreshhold;

    // This function runs the sobel algorithm over the opaque texture
    // void ColorSobel_float(float2 UV, float Thickness, out float Out) {
    //     // We have to run the sobel algorithm over the RGB channels separately
    //     float2 sobelR = 0;
    //     float2 sobelG = 0;
    //     float2 sobelB = 0;
    //     // We can unroll this loop to make it more efficient
    //     // The compiler is also smart enough to remove the i=4 iteration, which is always zero
    //     [unroll] for (int i = 0; i < 9; i++) {
    //         // Sample the scene color texture
    //         float3 rgb = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV + sobelSamplePoints[i] * Thickness);
    //         // Create the kernel for this iteration
    //         float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);
    //         // Accumulate samples for each color
    //         sobelR += rgb.r * kernel;
    //         sobelG += rgb.g * kernel;
    //         sobelB += rgb.b * kernel;
    //     }
    //     // Get the final sobel value
    //     // Combine the RGB values by taking the one with the largest sobel value
    //     Out = max(length(sobelR), max(length(sobelG), length(sobelB)));
    //     // This is an alternate way to combine the three sobel values by taking the average
    //     // See which one you like better
    //     //Out = (length(sobelR) + length(sobelG) + length(sobelB)) / 3.0;
    // }

    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
        float depth = LoadCameraDepth(varyings.positionCS.xy);
        PositionInputs posInput = GetPositionInput(varyings.positionCS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
        float3 viewDirection = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
        float4 color = float4(0.0, 0.0, 0.0, 0.0);

        // Load the camera color buffer at the mip 0 if we're not at the before rendering injection point
        if (_CustomPassInjectionPoint != CUSTOMPASSINJECTIONPOINT_BEFORE_RENDERING)
            color = float4(CustomPassLoadCameraColor(varyings.positionCS.xy, 0), 1);

        // Add your custom pass code here
        float2 sobel = 0;

        float2 sobelR = 0;
        float2 sobelG = 0;
        float2 sobelB = 0;

        
        [unroll] for (int i = 0; i < 9; i++) {
            
            float sdepth = LoadCustomDepth(posInput.positionSS + sobelSamplePoints[i] * _Thickness);
            
            sobel += sdepth * float2(sobelXMatrix[i], sobelYMatrix[i]);

            // Sample the scene color texture
            float3 rgb = CustomPassLoadCameraColor(varyings.positionCS.xy + sobelSamplePoints[i] * _Thickness, 0);
            // Create the kernel for this iteration
            float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);
            // Accumulate samples for each color
            sobelR += rgb.r * kernel;
            sobelG += rgb.g * kernel;
            sobelB += rgb.b * kernel;
        }
        float ColorOutlineStren = pow(smoothstep(0.0, _ColThreshhold,max(length(sobelR), max(length(sobelG), length(sobelB)))), _ColThickness) * _ColStr;
        float OutlineStren = pow(smoothstep(0.0, _DpthThreshhold,length(sobel)), _DpthThickness) * _DpthStr;
        
        // Fade value allow you to increase the strength of the effect while the camera gets closer to the custom pass volume
        float f = 1 - abs(_FadeValue * 2 - 1);
        return float4(lerp(color.rgb, _Color.rgb, floor(max(OutlineStren, ColorOutlineStren))) , 1.0);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Custom Pass 0"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
