Shader "WonderfulWorld/postprocess/hgsobelbuffer" {
	Properties{
		_Color("Color", Vector) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "black" {}
	}
		Fallback "Diffuse"
}