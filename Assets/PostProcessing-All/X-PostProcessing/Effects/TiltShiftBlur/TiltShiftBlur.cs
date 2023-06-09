
//----------------------------------------------------------------------------------------------------------
// X-PostProcessing Library
// https://github.com/QianMo/X-PostProcessing-Library
// Copyright (C) 2020 QianMo. All rights reserved.
// Licensed under the MIT License 
// You may not use this file except in compliance with the License.You may obtain a copy of the License at
// http://opensource.org/licenses/MIT
//----------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
////using UnityEngine.Rendering;
using RenderTargetIdentifier = UnityEngine.Rendering.RenderTargetIdentifier;
using UnityEngine.Rendering.PostProcessing;


namespace XPostProcessing
{
    public enum TiltShiftBlurQualityLevel
    {
        High_Quality = 0,
        Normal_Quality = 1,
    }

    [Serializable]
    public sealed class TiltShiftBlurQualityLevelParameter : ParameterOverride<TiltShiftBlurQualityLevel> { }

    [Serializable]
    [PostProcess(typeof(TiltShiftBlurRenderer), PostProcessEvent.AfterStack, "X-PostProcessing/Blur/TiltShiftBlur/TiltShiftBlurV1")]
    public class TiltShiftBlur : PostProcessEffectSettings
    {
        public TiltShiftBlurQualityLevelParameter QualityLevel = new TiltShiftBlurQualityLevelParameter { value = TiltShiftBlurQualityLevel.High_Quality };

        [Range(0.0f, 1.0f)]
        public FloatParameter AreaSize = new FloatParameter { value = 0.5f };

        [Range(0.0f, 1.0f)]
        public FloatParameter BlurRadius = new FloatParameter { value = 1.0f };

        [Range(1, 8)]
        public IntParameter Iteration = new IntParameter { value = 2 };

        [Range(1, 2)]
        public FloatParameter RTDownScaling = new FloatParameter { value = 1.0f };
    }

    public sealed class TiltShiftBlurRenderer : PostProcessEffectRenderer<TiltShiftBlur>
    {

        private const string PROFILER_TAG = "X-TiltShiftBlur";
        private Shader shader;


        public override void Init()
        {
            shader = Shader.Find("Hidden/X-PostProcessing/TiltShiftBlur");
        }

        public override void Release()
        {
            base.Release();
        }

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BlurredTex = Shader.PropertyToID("_BlurredTex");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
            internal static readonly int BufferRT2 = Shader.PropertyToID("_BufferRT2");
        }

        public override void Render(PostProcessRenderContext context)
        {

           UnityEngine.Rendering.CommandBuffer cmd =context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);


            if (settings.Iteration == 1)
            {
                HandleOneBlitBlur(context, cmd, sheet);
            }
            else
            {
                HandleMultipleIterationBlur(context, cmd, sheet, settings.Iteration);
            }

            cmd.EndSample(PROFILER_TAG);
        }


        void HandleOneBlitBlur(PostProcessRenderContext context, UnityEngine.Rendering.CommandBuffer cmd, PropertySheet sheet)
        {
            if (context == null || cmd == null || sheet == null)
            {
                return;
            }

            // Get RT
            int RTWidth = (int)(context.screenWidth / settings.RTDownScaling);
            int RTHeight = (int)(context.screenHeight / settings.RTDownScaling);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            // Set Property
            sheet.properties.SetVector(ShaderIDs.Params, new Vector2(settings.AreaSize, settings.BlurRadius));

            // Do Blit
            context.command.BlitFullscreenTriangle(context.source, ShaderIDs.BufferRT1, sheet, (int)settings.QualityLevel.value);

            // Final Blit
            cmd.SetGlobalTexture(ShaderIDs.BlurredTex, ShaderIDs.BufferRT1);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 2);

            // Release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
        }


        void HandleMultipleIterationBlur(PostProcessRenderContext context, UnityEngine.Rendering.CommandBuffer cmd, PropertySheet sheet, int Iteration)
        {
            if (context == null || cmd == null || sheet == null)
            {
                return;
            }

            // Get RT
            int RTWidth = (int)(context.screenWidth / settings.RTDownScaling);
            int RTHeight = (int)(context.screenHeight / settings.RTDownScaling);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT2, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            // Set Property
            sheet.properties.SetVector(ShaderIDs.Params, new Vector2(settings.AreaSize, settings.BlurRadius));

            RenderTargetIdentifier finalBlurID = ShaderIDs.BufferRT1;
            RenderTargetIdentifier firstID = context.source;
            RenderTargetIdentifier secondID = ShaderIDs.BufferRT1;
            for (int i = 0; i < Iteration; i++)
            {
                // Do Blit
                context.command.BlitFullscreenTriangle(firstID, secondID, sheet, (int)settings.QualityLevel.value);

                finalBlurID = secondID;
                firstID = secondID;
                secondID = (secondID == ShaderIDs.BufferRT1) ? ShaderIDs.BufferRT2 : ShaderIDs.BufferRT1;
            }

            // Final Blit
            cmd.SetGlobalTexture(ShaderIDs.BlurredTex, finalBlurID);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 2);

            // Release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT2);
        }

    }
}

