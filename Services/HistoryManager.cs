using System;
using System.Collections.Generic;
using FractalVision.Models;

namespace FractalVision.Services
{
    public class HistoryManager
    {
        private readonly Stack<FractalParameters> _undoStack;
        private readonly Stack<FractalParameters> _redoStack;
        private FractalParameters _current;
        private readonly int _maxHistorySize;

        public event Action HistoryChanged;
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public HistoryManager(FractalParameters initial, int maxHistorySize = 50)
        {
            _undoStack = new Stack<FractalParameters>();
            _redoStack = new Stack<FractalParameters>();
            _current = initial.Clone();
            _maxHistorySize = maxHistorySize;
        }

        public void AddState(FractalParameters parameters)
        {
            // Не добавляем если состояние почти идентично текущему
            if (_undoStack.Count > 0 &&
                IsSimilar(_undoStack.Peek(), parameters))
                return;

            _undoStack.Push(_current.Clone());
            _current = parameters.Clone();
            _redoStack.Clear();

            // Ограничиваем размер истории
            if (_undoStack.Count > _maxHistorySize)
            {
                var temp = new Stack<FractalParameters>();
                for (int i = 0; i < _maxHistorySize - 1; i++)
                    temp.Push(_undoStack.Pop());
                _undoStack.Clear();
                while (temp.Count > 0)
                    _undoStack.Push(temp.Pop());
            }

            HistoryChanged?.Invoke();
        }

        public FractalParameters Undo()
        {
            if (!CanUndo) return _current;

            _redoStack.Push(_current.Clone());
            _current = _undoStack.Pop();
            HistoryChanged?.Invoke();

            return _current.Clone();
        }

        public FractalParameters Redo()
        {
            if (!CanRedo) return _current;

            _undoStack.Push(_current.Clone());
            _current = _redoStack.Pop();
            HistoryChanged?.Invoke();

            return _current.Clone();
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            HistoryChanged?.Invoke();
        }

        public void UpdateCurrent(FractalParameters parameters)
        {
            _current = parameters.Clone();
        }

        private bool IsSimilar(FractalParameters a, FractalParameters b, double tolerance = 0.001)
        {
            return Math.Abs(a.CenterX - b.CenterX) < tolerance &&
                   Math.Abs(a.CenterY - b.CenterY) < tolerance &&
                   Math.Abs(a.Zoom - b.Zoom) < tolerance;
        }

        public string GetHistoryInfo()
        {
            return $"История: {_undoStack.Count} -> {_redoStack.Count}";
        }
    }
}