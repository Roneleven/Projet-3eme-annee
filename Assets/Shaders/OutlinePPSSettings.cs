// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( OutlinePPSRenderer ), PostProcessEvent.AfterStack, "Outline", true )]
public sealed class OutlinePPSSettings : PostProcessEffectSettings
{
	[Tooltip( "Color 0" )]
	public ColorParameter _Color0 = new ColorParameter { value = new Color(1f,1f,1f,0f) };
	[Tooltip( "Scale Bias" )]
	public FloatParameter _ScaleBias = new FloatParameter { value = 0f };
}

public sealed class OutlinePPSRenderer : PostProcessEffectRenderer<OutlinePPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "Outline" ) );
		sheet.properties.SetColor( "_Color0", settings._Color0 );
		sheet.properties.SetFloat( "_ScaleBias", settings._ScaleBias );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
