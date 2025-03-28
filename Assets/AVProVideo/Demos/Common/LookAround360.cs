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
		
		// 要播放的媒体URL
		private string mediaUrl = "http://192.168.5.22:18000/hls/Y5jQyJTNg/Y5jQyJTNg_live.m3u8";
		
		
		// 是否在启动时自动播放
		private bool autoPlayOnStart = false;
		
		// 添加ApplyToMesh引用，用于控制材质
		private ApplyToMesh applyToMesh;

		void Start()
		{
			// 获取ApplyToMesh组件
			//applyToMesh = GetComponent<ApplyToMesh>();
			
			// 设置Android平台选项
			mediaPlayer.PlatformOptionsAndroid.videoApi = Android.VideoApi.ExoPlayer;
			
			// 如果设置了自动播放，则开始播放
			if (autoPlayOnStart)
			{
				PlayMedia();
			}
			else
			{
				// 默认隐藏或设置材质为黑色
				// if (applyToMesh != null)
				// {
				// 	// 可选：创建并设置黑色材质
				// 	 applyToMesh.enabled = false;
				// }
			}
		}
		
		// 开始播放媒体
		public void PlayMedia()
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, mediaUrl, true);
				
				// 确保球体可见
				//gameObject.SetActive(true);
				
				// 启用ApplyToMesh组件
				// if (applyToMesh != null)
				// {
				// 	applyToMesh.enabled = true;
				// }
				
				Debug.Log("Media playback started: " + mediaUrl);
			}
		}
		
		// 停止播放媒体
		public void StopMedia()
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.CloseMedia();
				
				// 禁用ApplyToMesh组件
				// if (applyToMesh != null)
				// {
				// 	applyToMesh.enabled = false;
				// }
				
				Debug.Log("Media playback stopped");
			}
		}

		void Update()
		{

		}

		void OnDestroy()
		{
			// 确保在销毁时关闭媒体
			if (mediaPlayer != null)
			{
				mediaPlayer.CloseMedia();
			}
		}
	}
}