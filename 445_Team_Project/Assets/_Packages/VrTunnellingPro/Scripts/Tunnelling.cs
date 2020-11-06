using UnityEngine;
using System.Collections;
using Sigtrap.VrTunnellingPro;

namespace Sigtrap.VrTunnellingPro {
	/// <summary>
	/// The main Tunnelling effect.<br />
	/// This script uses post-processing.
	/// </summary>
	public class Tunnelling : TunnellingImageBase {
		protected override UnityEngine.Rendering.CameraEvent _maskCmdEvt {
			get {return UnityEngine.Rendering.CameraEvent.BeforeImageEffects;}
		}

		void OnRenderImage(RenderTexture src, RenderTexture dest){
			Draw(src, dest);
		}

        public void SetForceMode(bool enabled, float val)
        {
            _debugForceOn = enabled;
            _debugForceValue = val;
        }
	}
}