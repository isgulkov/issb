using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace issb
{
    /// <summary>
    /// Визуальный декоратор (adorner), добавляющий рабочему холсту поддержку множественного выделения с визуальной обратной связью
    /// </summary>
    public class MultipleSelectionAdorner : Adorner
    {
        /// <summary>
        /// Точки противоположных углов прямоугольника зоны выделения
        /// </summary>
        private Point? SelectionStartPoint, SelectionEndPoint;

        /// <summary>
        /// Визуальный элемент, выглядящий как прямоугольник, предназначенный для визуального отображения текущей зоны выделения на холсте
        /// </summary>
        private Rectangle SelectionRect;

        /// <summary>
        /// Рабочий холст, с которым рабоатет данный визуальный декоратор
        /// </summary>
        private StoryboardCanvas Canvas;

        /// <summary>
        /// Коллекция дочерних элементов данного визуального декоратора
        /// </summary>
        private VisualCollection Visuals;

        /// <summary>
        /// Холст данного визуального декоратора, на котором отображается зона выделения
        /// </summary>
        private Canvas AdornerCanvas;

        /// <summary>
        /// Возвращает число дочерних элементов данного визуального декоратора (необходимо для реализации визуального декоратора в WPF)
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return Visuals.Count;
            }
        }

        /// <summary>
        /// Конструктор. Объект данного типа предполагается создавать всякий раз, когда пользователь начинает операцию множественного выделения (путем перетаскивания указателя мыши по холсту с зажатой левой кнопкой)
        /// </summary>
        /// <param name="canvas">Холст, на который предполагается добавить данный визуальный декоратор</param>
        /// <param name="dragStartPoint">Начальная точка зоны выделения</param>
        public MultipleSelectionAdorner(StoryboardCanvas canvas, Point? dragStartPoint) : base(canvas)
        {
            Canvas = canvas;
            SelectionStartPoint = dragStartPoint;

            AdornerCanvas = new Canvas();
            AdornerCanvas.Background = Brushes.Transparent;
            Visuals = new VisualCollection(this);
            Visuals.Add(AdornerCanvas);

            SelectionRect = new Rectangle();
            SelectionRect.Stroke = Brushes.DarkGreen;
            SelectionRect.StrokeThickness = 1;
            SelectionRect.StrokeDashArray = new DoubleCollection(new double[] { 4 });

            AdornerCanvas.Children.Add(SelectionRect);
        }

        /// <summary>
        /// Обратаывает перемещение пользователем мыши по холсту, обновляя при этом как визуальное отображение зоны выделения, так и множество выделенных элементов раскадровки на холсте
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnMouseMove(MouseEventArgs eventArgs)
        {
            if(eventArgs.LeftButton == MouseButtonState.Pressed) {
                if(!IsMouseCaptured) {
                    CaptureMouse();
                }

                SelectionEndPoint = eventArgs.GetPosition(this);
                UpdateSelectionRect();
                UpdateSelection();
                eventArgs.Handled = true;
            }
        }

        /// <summary>
        /// Обрабатывает отпускание пользователем мыши, удаляя визуальные элементы, отображающие зону выделения
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnMouseUp(MouseButtonEventArgs eventArgs)
        {
            if(IsMouseCaptured) {
                ReleaseMouseCapture();
            }

            AdornerLayer adornerLayer = Parent as AdornerLayer;
            if(adornerLayer != null) {
                adornerLayer.Remove(this);
            }
        }

        /// <summary>
        /// Перестраивает в пространстве дочерние элементы данного визуального декоратора при перестроении его его родительскими элементом (необходимо для реализации визуального декоратора в WPF)
        /// </summary>
        /// <param name="arrangeBounds">Границы расположения элемента</param>
        /// <returns>Границы расположения элемента</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            AdornerCanvas.Arrange(new Rect(arrangeBounds));

            return arrangeBounds;
        }

        /// <summary>
        /// Возвращает дочерний элемент с переданным индексом (необходимо для реализации визуального декоратора в WPF)
        /// </summary>
        /// <param name="index">Индекс дочернего элемента</param>
        /// <returns>Дочерний элемент с переданным индексом</returns>
        protected override Visual GetVisualChild(int index)
        {
            return Visuals[index];
        }

        /// <summary>
        /// Обновляет визуальный элемент зоны выделения в соответствие с текущими точками противоположных углов зоны выделения
        /// </summary>
        private void UpdateSelectionRect()
        {
            double left = Math.Min(SelectionStartPoint.Value.X, SelectionEndPoint.Value.X);
            double top = Math.Min(SelectionStartPoint.Value.Y, SelectionEndPoint.Value.Y);

            double width = Math.Abs(SelectionStartPoint.Value.X - SelectionEndPoint.Value.X);
            double height = Math.Abs(SelectionStartPoint.Value.Y - SelectionEndPoint.Value.Y);

            SelectionRect.Width = width;
            SelectionRect.Height = height;

            System.Windows.Controls.Canvas.SetLeft(SelectionRect, left);
            System.Windows.Controls.Canvas.SetTop(SelectionRect, top);
        }

        /// <summary>
        /// Обновляет множество выделенных элементов на рабочем холсте
        /// </summary>
        private void UpdateSelection()
        {
            Rect selectionRect = new Rect(SelectionStartPoint.Value, SelectionEndPoint.Value);

            foreach(UIElement element in Canvas.Children) {
                if(!(element is StoryboardItem)) {
                    continue;
                }

                StoryboardItem storyboardItem = (StoryboardItem)element;

                Rect itemRect = VisualTreeHelper.GetDescendantBounds(storyboardItem);
                Rect itemBounds = storyboardItem.TransformToAncestor(Canvas).TransformBounds(itemRect);

                if(selectionRect.Contains(itemBounds)) {
                    storyboardItem.IsSelected = true;
                }
                else {
                    storyboardItem.IsSelected = false;
                }
            }
        }
    }
}
