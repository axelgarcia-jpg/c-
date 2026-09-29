using System;
using System.Numerics;
using Raylib_cs;

namespace ColorLab
{
    public struct ColorPaletteItem
    {
        public string Name;
        public Color Color;

        public ColorPaletteItem(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }

    class Program
    {
        // Función requerida para la conversión Hexadecimal
        static string ColorAHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        static void Main(string[] args)
        {
            const int windowWidth = 960;
            const int windowHeight = 680;

            Raylib.InitWindow(windowWidth, windowHeight, "Practica 3 - Color Lab");
            Raylib.SetTargetFPS(60);

            // Paleta temática de 5 colores[cite: 1]
            ColorPaletteItem[] palette = new ColorPaletteItem[]
            {
                new ColorPaletteItem("Rojo coral",    new Color(220, 50,  50,  255)),
                new ColorPaletteItem("Azul profundo", new Color(30,  80,  180, 255)),
                new ColorPaletteItem("Verde hoja",    new Color(40,  170, 90,  255)),
                new ColorPaletteItem("Amarillo sol",  new Color(245, 200, 40,  255)),
                new ColorPaletteItem("Morado",        new Color(140, 80,  180, 255))
            };

            int colorIndexA = 0; // Selección Clic Izquierdo
            int colorIndexB = 1; // Selección Clic Derecho 
            byte alphaValue = 255;
            bool isDraggingSlider = false;

            Rectangle sliderBounds = new Rectangle(430, 395, 380, 20);

            while (!Raylib.WindowShouldClose())
            {
                Vector2 mousePos = Raylib.GetMousePosition();

                // paleta de colores
                for (int i = 0; i < palette.Length; i++)
                {
                    Rectangle cardRect = new Rectangle(30, 120 + i * 65, 300, 55);
                    if (Raylib.CheckCollisionPointRec(mousePos, cardRect))
                    {
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) colorIndexA = i;
                        if (Raylib.IsMouseButtonPressed(MouseButton.Right)) colorIndexB = i;
                    }
                }

                // Control del Slider 
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mousePos, sliderBounds))
                {
                    isDraggingSlider = true;
                }
                if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                {
                    isDraggingSlider = false;
                }

                if (isDraggingSlider)
                {
                    float clampedX = Math.Clamp(mousePos.X, 430, 810);
                    alphaValue = (byte)Math.Round(((clampedX - 430) / 380.0f) * 255.0f);
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(242, 244, 248, 255));

                // Encabezado
                Raylib.DrawText("COLOR LAB", 30, 20, 28, new Color(30, 30, 30, 255));
                Raylib.DrawText("Selecciona un color y haz clic en el rectangulo para pintarlo.", 30, 55, 16, new Color(90, 90, 90, 255));

                // Panel Izquierdo
                Raylib.DrawText("Paleta de cinco colores", 30, 92, 18, new Color(30, 30, 30, 255));
                for (int i = 0; i < palette.Length; i++)
                {
                    int cardX = 30;
                    int cardY = 120 + i * 65;
                    Rectangle cardRect = new Rectangle(cardX, cardY, 300, 55);

                    bool isSelectedA = (colorIndexA == i);
                    bool isSelectedB = (colorIndexB == i);

                    Color cardBg = isSelectedA ? new Color(225, 236, 248, 255) : Color.White;
                    Raylib.DrawRectangleRec(cardRect, cardBg);
                    Raylib.DrawRectangleLinesEx(cardRect, 1, isSelectedA ? new Color(70, 130, 200, 255) : new Color(200, 200, 200, 255));

                    Raylib.DrawRectangle(cardX + 10, cardY + 8, 38, 38, palette[i].Color);
                    Raylib.DrawRectangleLines(cardX + 10, cardY + 8, 38, 38, new Color(0, 0, 0, 40));

                    Raylib.DrawText(palette[i].Name, cardX + 58, cardY + 8, 16, new Color(30, 30, 30, 255));
                    string rgbText = $"RGB({palette[i].Color.R}, {palette[i].Color.G}, {palette[i].Color.B})  {ColorAHex(palette[i].Color)}";
                    Raylib.DrawText(rgbText, cardX + 58, cardY + 28, 12, new Color(100, 100, 100, 255));

                    if (isSelectedA) Raylib.DrawText("A", cardX + 282, cardY + 8, 14, new Color(30, 80, 180, 255));
                    if (isSelectedB) Raylib.DrawText("B", cardX + 282, cardY + 30, 14, new Color(180, 50, 50, 255));
                }

                Raylib.DrawText("Izquierdo: seleccionar color / A", 30, 460, 13, new Color(100, 100, 100, 255));
                Raylib.DrawText("Derecho: elegir color B", 30, 480, 13, new Color(100, 100, 100, 255));

                // Panel Derecho:
                Raylib.DrawText("Rectangulo para pintar", 380, 92, 20, new Color(30, 30, 30, 255));
                int canvasX = 380, canvasY = 120, canvasW = 520, canvasH = 220;

                // Fondo dividido: Fondo claro (izquierda) y Fondo oscuro (derecha)
                Raylib.DrawRectangle(canvasX, canvasY, canvasW / 2, canvasH, new Color(240, 240, 240, 255));
                Raylib.DrawRectangle(canvasX + canvasW / 2, canvasY, canvasW / 2, canvasH, new Color(40, 40, 40, 255));

                // Patrón de damero
                int checkSize = 20;
                for (int cy = canvasY; cy < canvasY + canvasH; cy += checkSize)
                {
                    for (int cx = canvasX; cx < canvasX + canvasW; cx += checkSize)
                    {
                        if (((cx / checkSize) + (cy / checkSize)) % 2 == 0)
                        {
                            Raylib.DrawRectangle(cx, cy, checkSize, checkSize, new Color(220, 220, 220, 100));
                        }
                    }
                }

                // Renderizado del color activo 
                Color activeColorA = palette[colorIndexA].Color;
                Color displayColor = new Color(activeColorA.R, activeColorA.G, activeColorA.B, alphaValue);
                Raylib.DrawRectangle(canvasX, canvasY, canvasW, canvasH, displayColor);
                Raylib.DrawRectangleLines(canvasX, canvasY, canvasW, canvasH, new Color(100, 100, 100, 255));

                // Slider de Transparencia
                Raylib.DrawText("Transparencia (alpha)", 380, 360, 18, new Color(30, 30, 30, 255));
                int sliderX = 430, sliderY = 400, sliderWidth = 380;
                Raylib.DrawText("0", sliderX - 15, sliderY - 6, 14, new Color(80, 80, 80, 255));
                Raylib.DrawText("255", sliderX + sliderWidth + 10, sliderY - 6, 14, new Color(80, 80, 80, 255));
                Raylib.DrawRectangle(sliderX, sliderY - 2, sliderWidth, 5, new Color(210, 210, 210, 255));

                float knobX = sliderX + (alphaValue / 255.0f) * sliderWidth;
                Raylib.DrawCircle((int)knobX, sliderY, 10, new Color(45, 95, 160, 255));
                Raylib.DrawText($"Alpha: {alphaValue}", sliderX + (sliderWidth / 2) - 35, sliderY + 20, 15, new Color(60, 60, 60, 255));

                // Degradado Interpolado entre A y B
                Color activeColorB = palette[colorIndexB].Color;
                Raylib.DrawText($"Degradado: {palette[colorIndexA].Name} -> {palette[colorIndexB].Name}", 380, 465, 18, new Color(30, 30, 30, 255));

                int steps = 8, swatchW = 58, swatchH = 55, swatchGap = 8, startX = 380, startY = 495;
                for (int i = 0; i < steps; i++)
                {
                    // Interpolación del valor t de 0.00 a 1.00
                    float t = i / (float)(steps - 1);
                    byte r = (byte)(activeColorA.R + (activeColorB.R - activeColorA.R) * t);
                    byte g = (byte)(activeColorA.G + (activeColorB.G - activeColorA.G) * t);
                    byte b = (byte)(activeColorA.B + (activeColorB.B - activeColorA.B) * t);

                    int posX = startX + i * (swatchW + swatchGap);
                    Raylib.DrawRectangle(posX, startY, swatchW, swatchH, new Color(r, g, b, (byte)255));
                    Raylib.DrawText($"{t:F2}", posX + 12, startY + swatchH + 8, 12, new Color(60, 60, 60, 255));
                    Raylib.DrawText($"{r},{g},{b}", posX + 2, startY + swatchH + 24, 10, new Color(100, 100, 100, 255));
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}