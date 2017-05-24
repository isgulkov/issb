using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

namespace issb
{
    /// <summary>
    /// Представляет собой рабочий холст, на котором во время работы программы отображаются элементы раскадровки и ее фон
    /// </summary>
    public class StoryboardCanvas : Canvas
    {
        /// <summary>
        /// Точка начала перетаскивания (запоминается для поддержки множественного выделения)
        /// </summary>
        private Point? DragStartPoint = null;

        /// <summary>
        /// Объект <see cref="BackgroundManager"/>, установленный в качестве менеджера фона для данного рабочего холста
        /// </summary>
        public BackgroundManager BackgroundManager { get; set; }

        /// <summary>
        /// Наибольший ZIndex среди элементов раскадровки (запоминается для поддержки изменения взаимного расположения элементов по оси Z во время работы программы)
        /// </summary>
        int MaximumZIndex = int.MinValue + 1;

        /// <summary>
        /// Выделенные на данный момент элементы раскадровки
        /// </summary>
        public IEnumerable<StoryboardItem> SelectedItems
        {
            get
            {
                return Children.OfType<StoryboardItem>().Where(x => x.IsSelected);
            }
        }

        /// <summary>
        /// Снимает выделения со всех элементов раскадровки, расположенных на данном рабочем холсте
        /// </summary>
        public void DeselectAll()
        {
            foreach(StoryboardItem selectedItem in SelectedItems) {
                selectedItem.IsSelected = false;
            }
        }

        /// <summary>
        /// Обрабатывает начало множественного выделения элементов раскадровки
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnMouseDown(MouseButtonEventArgs eventArgs)
        {
            base.OnMouseDown(eventArgs);

            if(eventArgs.Source == this) {
                DragStartPoint = new Point?(eventArgs.GetPosition(this));
                DeselectAll();
                eventArgs.Handled = true;
            }
        }

        /// <summary>
        /// Обрабатывает движение мыши при множественном выделении элементов раскадровки
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnMouseMove(MouseEventArgs eventArgs)
        {
            base.OnMouseMove(eventArgs);

            if(eventArgs.LeftButton != MouseButtonState.Pressed) {
                DragStartPoint = null;
            }

            if(DragStartPoint.HasValue) {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if(adornerLayer != null) {
                    MultipleSelectionAdorner adorner = new MultipleSelectionAdorner(this, DragStartPoint);
                    if(adorner != null) {
                        adornerLayer.Add(adorner);
                    }
                }

                eventArgs.Handled = true;
            }
        }

        /// <summary>
        /// Удаляет выделенные на данный момент элементы раскадровки
        /// </summary>
        public void DeleteSelectedItems()
        {
            for(int i = 0; i < Children.Count; i++) {
                StoryboardItem item = Children[i] as StoryboardItem;

                if(item != null) {
                    if(item.IsSelected) {
                        Children.RemoveAt(i--);
                    }
                }
            }
        }

        /// <summary>
        /// Обрабатывает перетаскивание на данный рабочий холст элементов раскадровки и изображений-фонов с панелей инструментов
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnDrop(DragEventArgs eventArgs)
        {
            base.OnDrop(eventArgs);

            string xamlString;

            Point position = eventArgs.GetPosition(this);

            if((xamlString = eventArgs.Data.GetData("STORYBOARD_ITEM") as string) != null) {
                FrameworkElement content = XamlReader.Load(XmlReader.Create(new StringReader(xamlString))) as FrameworkElement;

                if(content != null) {
                    StoryboardItem newItem = new StoryboardItem();
                    newItem.Content = content;

                    if(content.MinHeight != 0 && content.MinWidth != 0) {
                        newItem.Width = content.MinWidth * 2; ;
                        newItem.Height = content.MinHeight * 2;
                    }
                    else {
                        newItem.Width = 65;
                        newItem.Height = 65;
                    }

                    SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                    SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));

                    Children.Add(newItem);

                    DeselectAll();
                    newItem.IsSelected = true;
                }

                eventArgs.Handled = true;
            }
            else if((xamlString = eventArgs.Data.GetData("STORYBOARD_BACKGROUND") as string) != null) {
                Image content = XamlReader.Load(XmlReader.Create(new StringReader(xamlString))) as Image;

                if(content != null && BackgroundManager != null) {
                    BackgroundManager.AddImageAt(position, content.Source);
                }
            }
        }

        /// <summary>
        /// Перемещает переданный элемент раскадровки на передний план
        /// </summary>
        /// <param name="item">Элемент раскадровки, который предполагается переместить на передний план</param>
        public void BringToFront(StoryboardItem item)
        {
            SetZIndex(item, ++MaximumZIndex);
        }
    }
}
