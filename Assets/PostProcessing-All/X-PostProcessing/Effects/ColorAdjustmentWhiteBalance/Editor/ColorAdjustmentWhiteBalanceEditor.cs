﻿
//----------------------------------------------------------------------------------------------------------
// X-PostProcessing Library
// https://github.com/QianMo/X-PostProcessing-Library
// Copyright (C) 2020 QianMo. All rights reserved.
// Licensed under the MIT License 
// You may not use this file except in compliance with the License.You may obtain a copy of the License at
// http://opensource.org/licenses/MIT
//----------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace XPostProcessing
{
    [PostProcessEditor(typeof(ColorAdjustmentWhiteBalance))]
    public sealed class ColorAdjustmentWhiteBalanceEditor : PostProcessEffectEditor<ColorAdjustmentWhiteBalance>
    {



        SerializedParameterOverride temperature;
        SerializedParameterOverride tint;


        public override void OnEnable()
        {
            temperature = FindParameterOverride(x => x.temperature);
            tint = FindParameterOverride(x => x.tint);
        }

        public override string GetDisplayTitle()
        {
            return XPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(temperature);
            PropertyField(tint);
        }

    }
}
        
