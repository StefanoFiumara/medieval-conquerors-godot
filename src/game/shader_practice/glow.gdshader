shader_type canvas_item;
render_mode unshaded;

// --- GLOW CONTROLS ---
uniform float thickness : hint_range(0.0, 100.0) = 15.0;
uniform int blur_steps : hint_range(4, 32) = 16;
uniform vec3 glow_color_1 : source_color = vec3(1.0, 0.8, 0.0);

uniform float time_scale : hint_range(0.0, 2.0) = 1.0;
uniform float glow_opacity : hint_range(0.0, 1.0) = 0.8;

uniform bool border_clipping_fix = true;
uniform float aspect_ratio : hint_range(0.1, 10.0, 0.01) = 1.0;
uniform vec2 offset;

void vertex(){
	if (border_clipping_fix){
		vec2 o = (UV * 2.0 - 1.0);
		VERTEX += o * thickness - offset;
		VERTEX.x *= 1.0 + (aspect_ratio-1.0) * (thickness * TEXTURE_PIXEL_SIZE.x) * 2.0;
	}
}

vec4 tex(sampler2D sampler, vec2 uv){
	if (uv.x > 0.0 && uv.y > 0.0 && uv.x < 1.0 && uv.y < 1.0){
		return texture(sampler, uv);
	}
	return vec4(0.0);
}

void fragment(){
	vec2 o = offset / vec2(textureSize(TEXTURE, 0));
	vec2 uv = UV;
	uv -= vec2(0.5);
	if (border_clipping_fix){
		uv.x *= 1.0 + (aspect_ratio-1.0) * thickness * TEXTURE_PIXEL_SIZE.x * 2.0;
		uv *= (1.0 + (thickness * TEXTURE_PIXEL_SIZE * 2.0));
		uv -= o;
	}
	uv += vec2(0.5);
	
	// Convert pixel thickness to texture coordinates
	vec2 size = vec2(thickness) / vec2(textureSize(TEXTURE, 0));
	vec4 sprite_color = tex(TEXTURE, uv);
	
	// --- GLOW ACCUMULATION (BLUR CORE) ---
	float accumulated_alpha = 0.0;
	float total_weights = 0.0;
	
	// We sample multiple nested rings to create a soft linear falloff gradient
	int num_rings = 4; 
	
	for (int ring = 1; ring <= num_rings; ring++) {
		// Linear falloff weight: Inner rings contribute more than outer rings
		float weight = 1.0 - (float(ring - 1) / float(num_rings)); 
		float current_radius = float(ring) / float(num_rings);
		
		for (int i = 0; i < blur_steps; ++i) {
			float radians_step = float(i) * 3.14159265 / float(blur_steps) * 2.0;
			vec2 sample_offset = vec2(
				size.x * sin(radians_step) * aspect_ratio, 
				size.y * cos(radians_step)
			) * current_radius;
			
			// Accumulate the sample alphas weighted by distance
			accumulated_alpha += tex(TEXTURE, uv + o + sample_offset).a * weight;
			total_weights += weight;
		}
	}
	
	// Normalize the blur value between 0.0 and 1.0
	float glow_strength = clamp(accumulated_alpha / total_weights, 0.0, 1.0);
	
	// Boost the visibility curve of the glow so it's punchy near the card
	glow_strength = pow(glow_strength, 0.6); 

	// --- ANIMATION & COMPOSITING ---
	// 1. Calculate standard human eye luminance of the chosen color
    float luminance = dot(glow_color_1.rgb, vec3(0.299, 0.587, 0.114));
    
    // 2. Create a universally rich shadow target (a deep, dark royal violet/indigo)
    vec3 shadow_target = vec3(0.15, 0.05, 0.25);
    
    // 3. Mix your color toward that rich shadow target. 
    // Brighter colors (like yellow/green) shift less dramatically; darker ones (like red/blue) blend smoothly.
    vec3 dynamic_shift = mix(shadow_target, glow_color_1.rgb * 0.5, clamp(luminance * 1.5, 0.3, 1.0));
    vec4 glow_color_2 = vec4(clamp(dynamic_shift, 0.0, 1.0), 0.4);
	
	float animation = 0.5 + 0.5 * sin(TIME * time_scale * 5.0);
	vec4 animated_glow = mix(vec4(glow_color_1, 1.), glow_color_2, animation);
	
	// Mix sprite texture and glow color
	vec3 final_color = mix(animated_glow.rgb, sprite_color.rgb, sprite_color.a);
	
	// The glow only shows where the original card texture is less than solid
	float final_alpha = max(sprite_color.a, glow_strength * glow_opacity);
	
	COLOR = vec4(final_color, final_alpha);
}
