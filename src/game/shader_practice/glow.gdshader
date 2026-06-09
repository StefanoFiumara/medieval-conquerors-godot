shader_type canvas_item;

uniform vec4 glow_color_1: source_color = vec4(1.0);
uniform vec4 glow_color_2: source_color = vec4(1.0);
uniform float line_thickness: hint_range(0.0, 1.0) = 0.5;
const float max_line_thickness = 10.;
uniform float time_scale: hint_range(0., 2.) = 1.;
uniform float glow_intensity: hint_range(0.5, 5.0) = 2.0;
uniform float glow_opacity: hint_range(0.0, 1.0) = 0.8;
uniform float search_radius: hint_range(1.0, 12.0) = 8.0;

void fragment() {
	vec4 color = texture(TEXTURE, UV);
	vec2 size = TEXTURE_PIXEL_SIZE * line_thickness * max_line_thickness;
	
	// Step 1: Find the minimum distance to any opaque pixel
	// We sample a larger neighborhood for better glow falloff
	float min_distance = 1e10;
	
	for (float y = -search_radius; y <= search_radius; y += 0.5) {
		for(float x = -search_radius; x <= search_radius; x += 0.5) {
			// Skip the center pixel itself
			if (length(vec2(x, y)) < 0.1) continue;
			
			// Sample alpha at this offset
			float sample_alpha = texture(TEXTURE, UV + size * vec2(x, y)).a;
			
			// If this pixel is opaque (not transparent)
			if (sample_alpha > 0.1) {
				// Calculate distance from center to this pixel
				float dist = length(vec2(x, y));
				min_distance = min(min_distance, dist);
			}
		}
	}
	
	// Step 2: Calculate smooth glow strength based on distance
	// Use smoothstep for a smooth curve instead of hard bands
	float glow_strength = 0.;
	if (min_distance < 1e9) {
		// smoothstep creates a smooth interpolation from 1.0 (at distance 0) to 0.0 (at glow_intensity)
		glow_strength = smoothstep(glow_intensity, 0., min_distance);
	}
	
	// Step 3: Apply periodic animation
	float animation = 0.5 + 0.5 * sin(TIME * time_scale * 5.);
	
	// Step 4: Mix between the two glow colors (including alpha)
	vec4 glow_color = mix(glow_color_1, glow_color_2, animation);
	
	// Step 5: Apply opacity control and glow color alpha to glow strength
	glow_strength *= glow_opacity * glow_color.a;
	
	// Step 6: Composite the result
	COLOR = color;
	
	// Only show glow where original texture is transparent
	if (color.a != 1.) {
		COLOR = vec4(mix(glow_color.rgb, color.rgb, color.a), glow_strength);
	}
	
	
	// 
	
}
