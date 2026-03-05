import math

def srgb_to_linear(c):
    if c <= 0.04045:
        return c / 12.92
    else:
        return math.pow((c + 0.055) / 1.055, 2.4)

def linear_to_srgb(c):
    if c <= 0.0031308:
        return c * 12.92
    else:
        return 1.055 * math.pow(c, 1/2.4) - 0.055

def apply_theme_multipliers_srgb(r, g, b, darken_mult=0.788, sat_mult=0.821):
    import colorsys
    
    # Unity UI color picker uses sRGB hex values, but the engine stores them in Linear if Color Space is Linear.
    # The raw float values in the YAML file: are they sRGB or Linear?
    # Usually, Unity serializers store sRGB values for color properties and convert internally if needed.
    
    # 1. Apply multipliers directly to the original floats in HSV space
    h, s, v = colorsys.rgb_to_hsv(r, g, b)
    
    v = v * darken_mult
    s = s * sat_mult
    
    r_new, g_new, b_new = colorsys.hsv_to_rgb(h, s, v)
    
    # Assume they need to be entered into a Unity Color Picker (which expects sRGB space)
    # If the project is Linear, the Color Picker does the Linear conversion under the hood.
    # We just need to give it the correct sRGB Hex.
    return f'#{int(r_new*255):02X}{int(g_new*255):02X}{int(b_new*255):02X}'

colors = {
    'Water Base (Original Float)': (0.6039216, 0.9098039, 0.8432034),
    'Land Base (Original Float)': (1, 0.9490197, 0.86274517),
    'Grass Base (Original Float)': (0.9294118, 0.909804, 0.54901963)
}

print('Calculated Colors (Assuming Float is sRGB):')
for name, (r, g, b) in colors.items():
    print(f'{name}: {apply_theme_multipliers_srgb(r, g, b)}')

print('\nOriginal Colors (No Multiplier):')
for name, (r, g, b) in colors.items():
    print(f'{name}: #{int(r*255):02X}{int(g*255):02X}{int(b*255):02X}')
