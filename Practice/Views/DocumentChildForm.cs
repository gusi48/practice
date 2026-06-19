using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Practice.Patterns;
using Practice.Services;

namespace Practice.Views
{
    public partial class DocumentChildForm : Form
    {
        private bool _isChanged = false;
        private Stack<DocumentMemento> _undoStack = new Stack<DocumentMemento>();
        private Stack<DocumentMemento> _redoStack = new Stack<DocumentMemento>();
        private bool _isChangingViaUndoRedo = false;
        private AutosaveService _autosaveService = new AutosaveService();
        private Timer _autosaveTimer;

        private DataGridView _virtualGrid;
        private BindingSource _bindingSource;
        private BindingList<TextMetric> _bindingList;
        private bool _isGridVisible = false;

        public event EventHandler<string> ContentChangedObserver;

        public class TextMetric
        {
            public string Параметр { get; set; }
            public int Значение { get; set; }
            public TextMetric(string param, int value)
            {
                Параметр = param;
                Значение = value;
            }
        }

        public DocumentChildForm()
        {
            InitializeComponent();

            _bindingList = new BindingList<TextMetric>();
            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _bindingList;

            _virtualGrid = new DataGridView();
            _virtualGrid.DataSource = _bindingSource;
            _virtualGrid.Width = 250;
            _virtualGrid.Dock = DockStyle.Right;
            _virtualGrid.Visible = false;
            _virtualGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _virtualGrid.AllowUserToAddRows = false;
            _virtualGrid.RowHeadersVisible = false;

            this.Controls.Add(_virtualGrid);

            if (txtContent != null)
            {
                txtContent.BringToFront();
            }

            _undoStack.Push(new DocumentMemento(""));
            txtContent.TextChanged += TxtContent_TextChanged;

            _autosaveTimer = new Timer();
            _autosaveTimer.Interval = 30000;
            _autosaveTimer.Tick += (s, e) => _autosaveService.SaveBackup(txtContent.Text);
            _autosaveTimer.Start();
        }

        public void ToggleGridVisibility()
        {
            _isGridVisible = !_isGridVisible;
            _virtualGrid.Visible = _isGridVisible;
        }

        public string DocumentText
        {
            get { return txtContent.Text; }
            set { txtContent.Text = value; }
        }

        private void TxtContent_TextChanged(object sender, EventArgs e)
        {
            if (_isChangingViaUndoRedo) return;

            if (!_isChanged)
            {
                _isChanged = true;
                this.Text += "*";
            }

            if (_undoStack.Count == 0 || _undoStack.Peek().State != txtContent.Text)
            {
                _undoStack.Push(new DocumentMemento(txtContent.Text));
                _redoStack.Clear();
            }

            UpdateGridMetrics();

            ContentChangedObserver?.Invoke(this, txtContent.Text);
        }

        private void UpdateGridMetrics()
        {
            string text = txtContent.Text;
            int totalChars = text.Length;
            int words = text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            int lines = text.Split(new char[] { '\n' }).Length;

            _bindingList.Clear();
            _bindingList.Add(new TextMetric("Строки", lines));
            _bindingList.Add(new TextMetric("Слова", words));
            _bindingList.Add(new TextMetric("Символы", totalChars));
        }

        public void MarkAsSaved(string realFileName)
        {
            _isChanged = false;
            this.Text = realFileName;
        }

        public void OpenExistingDocument(string title, string content)
        {
            this.Text = title;
            txtContent.Text = content;
            _undoStack.Clear();
            _redoStack.Clear();
            _undoStack.Push(new DocumentMemento(content));
            UpdateGridMetrics();
        }

        public void UndoAction()
        {
            if (_undoStack.Count > 1)
            {
                _isChangingViaUndoRedo = true;

                DocumentMemento current = _undoStack.Pop();
                _redoStack.Push(current);

                txtContent.Text = _undoStack.Peek().State;
                txtContent.SelectionStart = txtContent.Text.Length;

                _isChangingViaUndoRedo = false;
                UpdateGridMetrics();
            }
        }

        public void RedoAction()
        {
            if (_redoStack.Count > 0)
            {
                _isChangingViaUndoRedo = true;

                DocumentMemento nextState = _redoStack.Pop();
                _undoStack.Push(nextState);

                txtContent.Text = nextState.State;
                txtContent.SelectionStart = txtContent.Text.Length;

                _isChangingViaUndoRedo = false;
                UpdateGridMetrics();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _autosaveTimer.Stop();
            _autosaveTimer.Dispose();
            base.OnFormClosing(e);
        }
    }
}