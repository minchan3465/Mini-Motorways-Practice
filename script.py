import colorsys

def convert_color(r, g, b, darken_mult=0.788, sat_mult=0.821):
    # Convert RGB to HSV
    h, s, v = colorsys.rgb_to_hsv(r, g, b)
    
    # Apply multipliers
    v = v * darken_mult
    s = s * sat_mult
    
    # Convert back to RGB
    r_new, g_new, b_new = colorsys.hsv_to_rgb(h, s, v)
    
    # Convert to 0-255 and format as hex
    return f'#{int(r_new*255):02X}{int(g_new*255):02X}{int(b_new*255):02X}'

colors = {
    'Water': (0.6039216, 0.9098039, 0.8432034),
    'Water Grid': (0.56287825, 0.8584906, 0.7599532),
    'Beach': (1, 0.9775943, 0.9372549),
    'Grass': (0.9294118, 0.909804, 0.54901963),
    'Land': (1, 0.9490197, 0.86274517),
    'Road Inner': (1, 1, 1),
    'Road Outline': (0.936, 0.88497657, 0.796536),
    'Building Group A': (0.98823535, 0.7490196, 0.35686275),
    'Building Group B': (0.43569016, 0.82239395, 0.94509804)
}

print('Converted Colors:')
for name, (r, g, b) in colors.items():
    print(f'{name}: {convert_color(r, g, b)}')
