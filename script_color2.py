import colorsys
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

def convert_linear_color(r, g, b, darken_mult=0.788, sat_mult=0.821):
    # Unity saves floats in the YAML file. If the project is Linear, these floats 
    # MIGHT represent Linear color values, not sRGB.
    # Let's see what happens if we treat them as Linear, convert to sRGB (for the color picker),
    # and apply the multipliers.
    
    # 1. Apply multipliers to the raw floats first (this is what the game does at runtime)
    h, s, v = colorsys.rgb_to_hsv(r, g, b)
    v = v * darken_mult
    s = s * sat_mult
    r_lin, g_lin, b_lin = colorsys.hsv_to_rgb(h, s, v)
    
    # 2. Convert resulting Linear color to sRGB for the Unity Editor hex input
    r_srgb = linear_to_srgb(r_lin)
    g_srgb = linear_to_srgb(g_lin)
    b_srgb = linear_to_srgb(b_lin)
    
    return f'#{int(r_srgb*255):02X}{int(g_srgb*255):02X}{int(b_srgb*255):02X}'

colors = {
    'Water Base': (0.6039216, 0.9098039, 0.8432034),
    'Water Grid': (0.56287825, 0.8584906, 0.7599532),
    'Beach': (1, 0.9775943, 0.9372549),
    'Grass': (0.9294118, 0.909804, 0.54901963),
    'Land': (1, 0.9490197, 0.86274517),
    'Road Inner': (1, 1, 1),
    'Road Outline': (0.936, 0.88497657, 0.796536),
    'Building Group A': (0.98823535, 0.7490196, 0.35686275),
    'Building Group B': (0.43569016, 0.82239395, 0.94509804)
}

print('Converted Colors (Assuming YAML Floats are Linear, Hex output for sRGB Color Picker):')
for name, (r, g, b) in colors.items():
    print(f'{name}: {convert_linear_color(r, g, b)}')
