shader_type canvas_item;

const vec2 center = vec2(0.5);
uniform float width: hint_range(0.0, 1.0) = 0.5;
uniform vec2 highlight_center;
uniform float highlight_alpha: hint_range(0.0, 1.0) = 0.5;
uniform vec3 main_color: source_color;
uniform vec3 highlight_color: source_color;
uniform float glow: hint_range(0.0, 1.);
uniform sampler2D noise: repeat_enable;
uniform float radius: hint_range(0.0, 1.0) = 0.5;
uniform float speed: hint_range(0.0, 10.0) = 1;
void vertex() {
	//VERTEX += normalize(VERTEX) * 50. * sin(TIME) * .1;
}

void fragment() {
	float main_circle = step(distance(UV, center), radius);
	float main_inner = smoothstep(width * 0.5, 1., distance(UV, center));
	float main_shape = main_circle * main_inner;
	
	// TODO: animate highlight_center around unit circle, adjustable speed
	vec2 highlight_center2 = center + .5 * vec2(cos(2. * speed * TIME), sin(2. * speed * TIME));
	
	float highlight_shape = pow(1. - distance(UV, highlight_center2), 3.) * main_shape * highlight_alpha;
	
	vec3 color = main_color * main_shape + highlight_shape * highlight_color;
	color *= vec3(1. + glow * 5.);
	
	float texture_alpha = texture(noise, UV + vec2(TIME * 0.1, 0.)).r;
	
	//COLOR.a = highlight;
	COLOR.a = (main_shape + highlight_shape) * texture_alpha;
	COLOR.rgb = color;
}
