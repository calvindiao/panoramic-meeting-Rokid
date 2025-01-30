using UnityEngine;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2015-2021 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Demos
{
	/// <summary>
	/// Rotate the transform (usually with Camera attached) to look around during playback of 360/180 videos.
	/// Unity will rotate the camera automatically if VR is enabled, in which case this script does nothing.
	/// Otherwise if there is a gyroscope it will be used, otherwise the mouse/touch can be used.
	/// </summary>
	public class LookAround360 : MonoBehaviour
	{


        public MediaPlayer mediaPlayer;


		void Start()
		{

            //string defaultContent = "http://192.168.5.12:18000/hls/dyasUoVNg/dyasUoVNg_live.m3u8";
            string defaultContent = "http://192.168.5.124:18000/hls/IhSmluvNg/IhSmluvNg_live.m3u8";
            mediaPlayer.PlatformOptionsAndroid.videoApi = Android.VideoApi.ExoPlayer;
            mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, defaultContent, true);		  


        }

		void Update()
		{

		}

		void OnDestroy()
		{

		}

	}
}