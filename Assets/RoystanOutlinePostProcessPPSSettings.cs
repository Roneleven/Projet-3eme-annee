// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( RoystanOutlinePostProcessPPSRenderer ), PostProcessEvent.AfterStack, "RoystanOutlinePostProcess", true )]
public sealed class RoystanOutlinePostProcessPPSSettings : PostProcessEffectSettings
{
}

public sealed class RoystanOutlinePostProcessPPSRenderer : PostProcessEffectRenderer<RoystanOutlinePostProcessPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "Hidden/Roystan/Outline Post Process" ) );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
