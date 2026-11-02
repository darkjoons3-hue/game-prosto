using System.Drawing.Text;

namespace SquareBlackAndGreen;

public class DisclaimerForm : Form
{
    private readonly PrivateFontCollection _fonts = new();
    private Font _gameFont;
    private Font _hintFont;

    private readonly Color _green = Color.FromArgb(0, 210, 0);
    private readonly Color _dimGreen = Color.FromArgb(0, 90, 0);

    private readonly string[] _lines =
    {
        "ЭТА ИГРА — НЕ ВИРУС.",
        "",
        "Она не нанесёт вреда вашему компьютеру.",
        "Но она БУДЕТ вас пугать.",
        "",
        "Она может имитировать:",
        "   доступ к вашей камере",
        "   открытие браузера",
        "   консольные команды",
        "   процессы в диспетчере задач",
        "   работу с файлами",
        "",
        "Всё это — часть истории.",
        "Продолжая, вы соглашаетесь.",
    };

    private int _currentLine;
    private int _currentChar;
    private bool _done;
    private bool _cursorVisible = true;

    private readonly System.Windows.Forms.Timer _typeTimer = new() { Interval = 28 };
    private readonly System.Windows.Forms.Timer _blinkTimer = new() { Interval = 500 };

    public DisclaimerForm()
    {
        Text = "ge";
        FormBorderStyle = FormBorderStyle.FixedSingle;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(920, 680);
        BackColor = Color.Black;
        ForeColor = _green;
        DoubleBuffered = true;
        MaximizeBox = false;
        KeyPreview = true;

        LoadFonts();

        _typeTimer.Tick += (_, _) => TypeTick();
        _blinkTimer.Tick += (_, _) => { _cursorVisible = !_cursorVisible; Invalidate(); };

        _typeTimer.Start();
        _blinkTimer.Start();
    }

    private void LoadFonts()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Monodetermination.ttf");
        if (File.Exists(path))
        {
            _fonts.AddFontFile(path);
            _gameFont = new Font(_fonts.Families[0], 22f);
            _hintFont = new Font(_fonts.Families[0], 14f);
        }
        else
        {
            // fallback, если шрифт не найден
            _gameFont = new Font("Consolas", 20f);
            _hintFont = new Font("Consolas", 13f);
        }
    }

    private void TypeTick()
    {
        if (_currentLine >= _lines.Length)
        {
            _typeTimer.Stop();
            _done = true;
            Invalidate();
            return;
        }

        if (_currentChar < _lines[_currentLine].Length)
        {
            _currentChar++;
        }
        else
        {
            _currentLine++;
            _currentChar = 0;
        }
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        using var brush = new SolidBrush(_green);
        float y = 50f;
        const float lineHeight = 34f;
        const float leftPad = 60f;

        for (int i = 0; i < _lines.Length; i++)
        {
            if (i > _currentLine) break;

            string line = _lines[i];
            if (i == _currentLine && !_done)
                line = line.Substring(0, _currentChar);

            // курсор в конце печатающейся строки
            if (i == _currentLine && !_done && _cursorVisible)
                line += "_";

            g.DrawString(line, _gameFont, brush, leftPad, y);
            y += lineHeight;
        }

        // подсказка внизу
        if (_done && _cursorVisible)
        {
            using var hintBrush = new SolidBrush(_dimGreen);
            string hint = "нажмите любую клавишу...";
            var size = g.MeasureString(hint, _hintFont);
            g.DrawString(hint, _hintFont, hintBrush,
                (ClientSize.Width - size.Width) / 2f,
                ClientSize.Height - 50f);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!_done)
        {
            // пропустить печать — сразу показать весь текст
            _currentLine = _lines.Length;
            _currentChar = 0;
            _typeTimer.Stop();
            _done = true;
            Invalidate();
        }
        else
        {
            // TODO: здесь будет переход в главное меню
            MessageBox.Show("Дальше будет главное меню.", "ge");
            Close();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _typeTimer.Dispose();
            _blinkTimer.Dispose();
            _gameFont.Dispose();
            _hintFont.Dispose();
            _fonts.Dispose();
        }
        base.Dispose(disposing);
    }
}
