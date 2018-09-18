Shader "Vertex Colors" {


	Subshader{
		BindChannels{
		Bind "vertex", vertex
		Bind "color", color
	}
		Pass{}
	}


}