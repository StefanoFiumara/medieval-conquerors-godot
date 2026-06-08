shader_type canvas_item;

uniform float dot_size: hint_range(0.0, 1.0) = 1;
uniform float ring_thickness: hint_range(0.0, 1.0) = 1;
uniform float cross_gap: hint_range(0.0, 1.0) = 1;
uniform float line_thickness: hint_range(0.0, 1.0) = 1;
uniform float line_gap: hint_range(0.0, 1.0) = 1;

float create_lines(vec2 uv) {
	float horizontal = step(distance(uv, vec2(uv.x ,0.5)), line_thickness * 0.02)
	- step(distance(uv.x, .5), line_gap * 0.5);
	float vertical = step(distance(uv, vec2(0.5 ,uv.y)), line_thickness * 0.02)
	- step(distance(uv.y, .5), line_gap * 0.5);
	
	return clamp(vertical, 0.,1.) + clamp(horizontal, 0.,1.);
}

float create_ring(vec2 uv) {
	float outer = step(distance(uv, vec2(0.5)), 0.5);
	float inner = step(distance(uv, vec2(0.5)), .5 - (ring_thickness * 0.5 * 0.15));
	float ring = outer - inner;
	
	float top    = step(distance(uv, vec2(0.5, 0.0)), cross_gap * 0.2);
	float bottom = step(distance(uv, vec2(0.5, 1.0)), cross_gap * 0.2);
	float left   = step(distance(uv, vec2(0.0, 0.5)), cross_gap * 0.2);
	float right  = step(distance(uv, vec2(1.0, 0.5)), cross_gap * 0.2);
	
	return clamp(ring - top - bottom - left - right, 0., 1.);
}

float create_middle_dot(vec2 uv) {
	return step(distance(uv, vec2(0.5)), dot_size * 0.045);
}

void fragment() {
	COLOR.rgb = vec3(1.);
	COLOR.a = create_lines(UV);
	COLOR.a = clamp(create_middle_dot(UV) + create_ring(UV) + create_lines(UV), 0., 1.);
}
