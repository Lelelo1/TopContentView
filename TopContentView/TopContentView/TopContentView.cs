using System;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System.Linq.Expressions;

namespace Control
{
    /* topContentViews will appear in the order they where given/added to parent view
     * with the latest one added added at the most top (Z-axis). 
    */
    public class TopContentView : ContentView
    {

        public String Name { get; set; } // for testing

        public TopContentView()
        {

            /* Xamarin.forms sets the VisualElement.X and VisualElement.Y internally
             * after they are set via TopContentView.X and TopContentView.Y.
             * Thus the X and Y values will not be set initially
            */
            this.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {

                if (e.PropertyName == "X")
                {
                    if (!double.Equals(x, X))
                    {
                        // why are this called 2 times?
                        // Console.WriteLine("x: " + x + " not equal to X: " + X);
                        X = x;

                    }
                }
                else if (e.PropertyName == "Y")
                {
                    // Console.WriteLine("y: " + y + " and " + "Y: " + Y);
                    if (!double.Equals(y, Y))
                    {
                        Y = y;
                    }
                }
                if (e.PropertyName == "Parent")
                {
                    // Console.WriteLine("Parent changed");
                    var layout = Parent as Layout<View>;

                    if (layout != null)
                    {
                        layout.LayoutChanged -= TopContentView_LayoutChanged;
                        layout.LayoutChanged += TopContentView_LayoutChanged;
                        var collection = (INotifyCollectionChanged)layout.Children;
                        collection.CollectionChanged -= Collection_CollectionChanged;
                        collection.CollectionChanged += Collection_CollectionChanged;
                        // consider handle removal of event handlers when parent is beging removed (propertychanging)
                    }
                }
                if(e.PropertyName == "Content")
                {
                    if(Content != null)
                    {
                        Listen(Content); // get notification when WidthRequest and heightRequest in any decendant control changes
                    }
                }
   
            };

            /* Needed to provide height for content when it was visibility initially was set to false */
            this.PropertyChanging += (object sender, Xamarin.Forms.PropertyChangingEventArgs e) =>
            {
                
                if(e.PropertyName == "IsVisible")
                {
                    if(!IsVisible) // changing to true
                    {
                        WidthRequest = -1;
                        HeightRequest = -1;
                    }
                }
                
            };
        }
        /* Needs testing with more complex decendant layouts and controls scenarios */
        VisualElement Listen(VisualElement visualElement)
        {
            visualElement.PropertyChanging += VisualElement_PropertyChanging;
            // target all layouts

            if(visualElement is Layout<View> layoutWithChildren)
            {

                foreach(var child in layoutWithChildren.Children)
                {
                    Listen(child);
                }
            }
            else if(visualElement is Layout layoutWithContent)
            {
                var property = layoutWithContent.GetType().GetProperty("Content");

                var get = layoutWithContent.GetType().GetProperty("Content").GetGetMethod();

                var content = (VisualElement) get.Invoke(layoutWithContent, new object[] { });
                Listen(content);
            }
            return visualElement;
        }
        
        private void VisualElement_PropertyChanging(object sender, Xamarin.Forms.PropertyChangingEventArgs e)
        {
            var visualElement = ((VisualElement)sender);

            if(e.PropertyName == "WidthRequest")
            {
                WidthRequest = -1;

            }
            if(e.PropertyName == "HeightRequest")
            {
                HeightRequest = -1;

            }
        }
        /* can run two times. This happens when verticaloptions/horizontaloptions is set on any control
        * inside the layout and app uses appshell/is shellcontent. (probably caused from navigationbar)
        * reproduce in new project, report bug.
        */
        /* many calls are made beacuse width/height requests need resetting to 0 to hide empty area.
        * Visibility and decendant width/height request is monitored manually with Listener and sets it -1
        * so that parent layout draws the content.
        */
        private double width = -1;
        private double height = -1;
        private void TopContentView_LayoutChanged(object sender, EventArgs e)
        {
            // Console.WriteLine("Layout changed: " + X + ", " + Y + ", " + Width + ", " + Height + ". WidthRequest: " + WidthRequest + ". HeightRequest: " + HeightRequest);
            var layout = (Layout<View>)sender;

            // Console.WriteLine("Rerender with new bounds");

            if (Width > 0)
            {
                width = Width;
            }
            if (Height > 0)
            {
                height = Height;
            }
            var w = WidthRequest;
            var h = HeightRequest;

            // flickering on topcontentview after adding a few labels -and sometimes dissappearing - has dissapeared

            // 0 removes empty area created in the original position by the layout but -1 is needed to get updated width and height
            WidthRequest = 0;
            HeightRequest = 0;
            
            // Console.WriteLine("width: " + width + ", height: " + height);
            Layout(new Rectangle(X, Y, width, height)); // makes the content

        }

        public static new BindableProperty XProperty =
            BindableProperty.Create("X", typeof(double), typeof(TopContentView), propertyChanged:
                (BindableObject bindable, object oldValue, object newValue) => (bindable as TopContentView).X = (double)newValue);

        private double x;
        public new double X
        {
            get
            {
                var property = typeof(VisualElement).GetProperty("X", BindingFlags.Instance | BindingFlags.Public);
                var get = property.GetGetMethod();
                var value = (double)get.Invoke(this, null);
                // Console.WriteLine("getting: " + value);
                return value;
            }
            set
            {
                this.x = value;
                var property = typeof(VisualElement).GetProperty("X", BindingFlags.Instance | BindingFlags.Public);
                var set = property.GetSetMethod(true);
                // Console.WriteLine("setting: " + value);
                set.Invoke(this, new object[] { value });
            }
        }
        public static new BindableProperty YProperty =
            BindableProperty.Create("Y", typeof(double), typeof(TopContentView), propertyChanged:
                (BindableObject bindable, object oldValue, object newValue) => (bindable as TopContentView).Y = (double)newValue);

        private double y;
        public new double Y
        {
            get
            {
                var property = typeof(VisualElement).GetProperty("Y", BindingFlags.Instance | BindingFlags.Public);
                var get = property.GetGetMethod();
                var value = (double)get.Invoke(this, null);
                // Console.WriteLine("getting: " + value);
                return value;
            }
            set
            {
                this.y = value;
                var property = typeof(VisualElement).GetProperty("Y", BindingFlags.Instance | BindingFlags.Public);
                var set = property.GetSetMethod(true); // nonpublic: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo.getsetmethod?view=netframework-4.8
                // Console.WriteLine("setting: " + value);
                set.Invoke(this, new object[] { value });
            }
        }

        /*
         * The following is used to maintain topcontentviews top of the visual stack
         * works - tested with many topcontenviews and adding removing controls on
         * layout. contentview stay in order they where specified (and they stay in the same cordinate as specified as well)
         * 
         * --> need test when moving topcontentviews between different layouts <--
        */
        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            // Console.WriteLine("CollectionChanged");
            var elementCollection = (IList<Element>)sender;
            var ordered = elementCollection.OrderBy((element) => element is TopContentView);

            if (!elementCollection.SequenceEqual(ordered))
            {

                /*
                 * Needed to prevent: Cannot change ObservableCollection during a CollectionChanged event
                 * error
                 * delays the changing of observablecollection - thus occuring after LayoutChanged event.
                 */
                /*
                Device.InvokeOnMainThreadAsync(() =>
                {
                    (Parent as Layout<View>).RaiseChild(this);
                });
                */
                Device.BeginInvokeOnMainThread(() =>
                {
                    (Parent as Layout<View>).RaiseChild(this);
                });
            }
        }
    }

    /*
     * Need to enable WidthRequest and HeightRequest - so they can be set and used as expected <--
     * Need to correct/adjust for when parent layout has spacing
    */

}
