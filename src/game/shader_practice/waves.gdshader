shader_type canvas_item;

uniform sampler2D scale_curve;
uniform float time_scale: hint_range(0., 2.) = 1.;

void vertex() {
	// use TIME to sample along curves
	float normalized_curve = 0.5 * (1. + sin(TIME)); // periodic 0 -> 1
	// use fract() to sample from 0 -> 1 and repeat without being periodic
	float linear_curve = fract(TIME * time_scale);
	
	// sample along curve over time 
	// curves are interpreted as one-dimensional textures
	float curve_output = texture(scale_curve, vec2(linear_curve, 0.)).r;
	VERTEX *= 1. + curve_output;
}
