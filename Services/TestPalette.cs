using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FractalVision.Enums;

namespace FractalVision.Services
{
    public static class TestPalette
    {
        public static void RunTest()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("=== ТЕСТ COLOR PALETTES ===\n");

            try
            {
                // Тест 1: Радужная палитра
                result.AppendLine("1. Радужная палитра:");
                var rainbow = new ColorPalette(ColorScheme.Rainbow);
                result.AppendLine($"   {rainbow.GetPaletteInfo()}");

                // Показываем несколько цветов
                result.AppendLine("   Примеры цветов:");
                result.AppendLine($"   Итерация 0: {rainbow.GetColor(0, 100).Name}");
                result.AppendLine($"   Итерация 25: {rainbow.GetColor(25, 100).Name}");
                result.AppendLine($"   Итерация 50: {rainbow.GetColor(50, 100).Name}");
                result.AppendLine($"   Итерация 75: {rainbow.GetColor(75, 100).Name}");
                result.AppendLine($"   Итерация -1: {rainbow.GetColor(-1, 100).Name} (черный)\n");

                // Тест 2: Огненная палитра
                result.AppendLine("2. Огненная палитра:");
                var fire = new ColorPalette(ColorScheme.Fire);
                result.AppendLine($"   {fire.GetPaletteInfo()}\n");

                // Тест 3: Океанская палитра
                result.AppendLine("3. Океанская палитра:");
                var ocean = new ColorPalette(ColorScheme.Ocean);
                result.AppendLine($"   {ocean.GetPaletteInfo()}\n");

                // Тест 4: Пользовательская палитра
                result.AppendLine("4. Пользовательская палитра (синий → белый):");
                var custom = new ColorPalette();
                custom.CreateCustomPalette(Color.Blue, Color.White);
                result.AppendLine($"   {custom.GetPaletteInfo()}\n");

                // Тест 5: Изменение количества цветов
                result.AppendLine("5. Тест изменения количества цветов:");
                var palette = new ColorPalette(ColorScheme.Rainbow);
                result.AppendLine($"   Изначально: {palette.ColorCount} цветов");

                palette.ColorCount = 128;
                result.AppendLine($"   После изменения: {palette.ColorCount} цветов");
                result.AppendLine($"   Информация: {palette.GetPaletteInfo()}\n");

                // Тест 6: Визуальный тест (создаем маленькую картинку)
                result.AppendLine("6. Визуальный тест градиентов:");

                // Создаем маленькое изображение для каждой палитры
                foreach (ColorScheme scheme in Enum.GetValues(typeof(ColorScheme)))
                {
                    if (scheme == ColorScheme.Custom) continue;

                    var testPalette = new ColorPalette(scheme);
                    result.AppendLine($"   {scheme}: {testPalette.Colors.Length} цветов в градиенте");
                }

                result.AppendLine("\n=== ВСЕ ПАЛИТРЫ РАБОТАЮТ ===");
            }
            catch (Exception ex)
            {
                result.AppendLine($"ОШИБКА: {ex.Message}");
            }

            MessageBox.Show(result.ToString(), "Тест ColorPalette",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}