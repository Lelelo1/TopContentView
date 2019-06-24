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

namespace TopContentView
{
    /* topContentViews will appear in the order they where given/added to parent view
     * with the latest one added added at the most top (Z-axis). 
    */
    public class TopContentView : ContentView
    {

        public String Name { get; set; } // for testing
        private bool HasRepaintedSiblings = false;
        public TopContentView()
        {

            /* Xamarin.forms sets the VisualElement.X and VisualElement.Y internally
             * after they are set via TopContentView.X and TopContentView.Y.
             * Thus the X and Y values will not be set initially
            */
            var t = this;

            this.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {

                if (e.PropertyName == "X")
                {
                    if (!double.Equals(x, X))
                    {
                        // why are this called 2 times?
                        Console.WriteLine("x: " + x + " not equal to X: " + X);
                        X = x;
                        // when moved 
                        var parent = Parent as Layout<View>;

                        if (parent != null)
                        {

                            /* Occurs when having two topContentViews defined in xaml
                             * To prevent System.InvalidOperationException with message
                             * Cannot change ObservableCollection during a CollectionChanged event.
                             */

                            // CollectionChanged and LayoutChanged not firing
                            /*
                            if (!HasRepaintedSiblings)
                            {
                                Console.WriteLine("removing and adding");
                                parent.Children.Remove(this);
                                parent.Children.Add(this);
                                HasRepaintedSiblings = true;
                            }
                            */

                        }
                    }
                }
                else if (e.PropertyName == "Y")
                {
                    Console.WriteLine("y: " + y + " and " + "Y: " + Y);
                    if (!double.Equals(y, Y))
                    {
                        Y = y;
                        var parent = Parent as Layout<View>;

                        if (parent != null)
                        {
                            if (!HasRepaintedSiblings)
                            {
                                /*
                                Console.WriteLine("removing and adding");
                                parent.Children.Remove(this);
                                parent.Children.Add(this);
                                HasRepaintedSiblings = true;
                                */
                                /*
                                List<View> l = new List<View>(parent.Children);
                                l.Remove(this);
                                l.Add(this);
                                parent.Children.Clear();
                                foreach(var v in l)
                                {
                                    parent.Children.Add(v);
                                }
                                HasRepaintedSiblings = true;
                                */
                            }
                        }

                    }
                }
                if (double.Equals(x, X) && double.Equals(y, Y))
                {
                    var parent = Parent as Layout<View>;

                    if (parent != null)
                    {
                        if (parent.Children != null && !HasRepaintedSiblings)
                        {
                            /*
                            try
                            {
                                parent.Children.Remove(this);
                                parent.Children.Add(this);

                                Console.WriteLine("Successfully removed and added");
                                HasRepaintedSiblings = true;
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message);
                            }
                            */
                            /*
                            AsyncEventListener whenLayoutChanged = new AsyncEventListener();
                            (this.Parent as Layout<View>).LayoutChanged += TopContentView_LayoutChanged;
                            */
                            // parent.LayoutChanged += TopContentView_LayoutChanged;

                        }
                        else
                        {
                            // parent.LayoutChanged -= TopContentView_LayoutChanged;
                        }
                    }

                }
                if (e.PropertyName == "Parent")
                {
                    // Console.WriteLine("Parent changed");
                    var layout = (Parent as Layout<View>);
                    var c = this; // no  bounds

                    if (layout != null)
                    {
                        layout.LayoutChanged -= TopContentView_LayoutChanged;
                        layout.LayoutChanged += TopContentView_LayoutChanged;

                        /* Children changed is not triggered: https://forums.xamarin.com/discussion/158703/how-can-i-intercept-children-added-to-layout/p1?new=1
                        layout.PropertyChanging += Layout_PropertyChanging;
                        layout.PropertyChanged += Layout_PropertyChanged;
                        // layout.ChildAdded += Layout_ChildAdded; does trigger
                        */
                        // layout.ChildrenReordered += Layout_ChildrenReordered;
                        // https://stackoverflow.com/questions/9743420/remove-an-item-from-an-observablecollection-in-a-collectionchanged-event-handler
                        var collection = layout.Children as INotifyCollectionChanged;
                        if (collection != null)
                        {
                            Console.WriteLine("assigned listener");
                            collection.CollectionChanged += Collection_CollectionChanged;
                            // collection.CollectionChanged += whenCollectionChanged.Listen;
                            // whenCollectionChanged.Then += WhenCollectionChanged_Then;
                        }
                    }

                }
            };

        }

        //  https://github.com/mrxten/XamEffects

        /* works - tested with many contenview and adding removing controlson
        * layout. contentview stay in order they where specified (and they stay in the same cordinate as specified as well)
        */
        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            /*
            Console.WriteLine("collectionchanged: " + eventArgs.ToString());
            var ObservableWrapperType = sender.GetType().BaseType;
            var field = ObservableWrapperType.GetField("_list", BindingFlags.Instance | BindingFlags.NonPublic);
            var value = field.GetValue(sender);
            Console.WriteLine("vacannolue: " + value);
            */


            // Console.WriteLine("collection: " + collection);
            var elementCollection = (IList<Element>)sender;
            var ordered = elementCollection.OrderBy((element) => element is TopContentView);

            if (!elementCollection.SequenceEqual(ordered))
            {
                /*
                 * Task.WhenAll(whileEvent).ContinueWith((completed) =>
                {

                    whileEvent.Add(Successfully);

                    Successfully.GetAwaiter().OnCompleted(() =>
                    {
                        Console.WriteLine(Successfully.Name + " finished");
                        whileEvent.Remove(Successfully);
                    });
                    Console.WriteLine("started " + Successfully.Name);
                    Successfully.Start();
                }).Wait();
                */
                /*
                await Task.WhenAll(tasks).ContinueWith((completed) =>
                {
                    Task task = Device.InvokeOnMainThreadAsync(() =>
                    {

                         Object of type 'System.Collections.ObjectModel.ObservableCollection`1[Xamarin.Forms.Element]'
                         doesn't match target type 'Xamarin.Forms.ObservableWrapper`2[Xamarin.Forms.Element,Xamarin.Forms.View]'

                        // does not have time to run - cancelled by preceding collectionchanged

                        var method = ObservableWrapperType.GetMethod("ListOnCollectionChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                        var @delegate = Delegate.CreateDelegate(typeof(Action), method);
                        var d = method.CreateDelegate(typeof(NotifyCollectionChangedEventHandler)) as NotifyCollectionChangedEventHandler;
                        collection.CollectionChanged += d;

                        Task.Delay(5000).Wait();
                        Console.WriteLine("yoo");
                        var orderedCollection = new ObservableCollection<Element>(ordered);

                        // orderedCollection.CollectionChanged += (object s, NotifyCollectionChangedEventArgs e) => method.Invoke(s, new object[] { s, e });
                        // collection.CollectionChanged += (object s, NotifyCollectionChangedEventArgs e) => method.Invoke(s, new object[] { s, e });

                        field.SetValue(sender, orderedCollection);
                    });
                    tasks.Add(task);
                    task.GetAwaiter().OnCompleted(() =>
                    {
                        tasks.Remove(task);
                    });
                });
                */
                /*
                 * Needed to prevent: Cannot change ObservableCollection during a CollectionChanged event
                 * error
                 * delays the changing of observablecollection - thus occuring after LayoutChanged event.
                 */
                Device.InvokeOnMainThreadAsync(() =>
                {
                    /*
                     * Object of type 'System.Collections.ObjectModel.ObservableCollection`1[Xamarin.Forms.Element]'
                     * doesn't match target type 'Xamarin.Forms.ObservableWrapper`2[Xamarin.Forms.Element,Xamarin.Forms.View]'
                     */
                    // does not have time to run - cancelled by preceding collectionchanged

                    //var method = ObservableWrapperType.GetMethod("ListOnCollectionChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                    // var @delegate = Delegate.CreateDelegate(typeof(Action), method); // breaks

                    // var listOnCollectionChanged = method.CreateDelegate(typeof(NotifyCollectionChangedEventHandler)) as NotifyCollectionChangedEventHandler;
                    // collection.CollectionChanged += d;

                    /*
                    Console.WriteLine("yoo");
                    var orderedCollection = new ObservableCollection<Element>(ordered);
                    var del = Delegate.CreateDelegate(type: typeof(NotifyCollectionChangedEventHandler), method: "ListOnCollectionChanged", target: sender); //target was needed to specify
                    orderedCollection.CollectionChanged += del as NotifyCollectionChangedEventHandler;
                    */

                    // orderedCollection.CollectionChanged += (object s, NotifyCollectionChangedEventArgs e) => method.Invoke(s, new object[] { s, e });
                    // collection.CollectionChanged += (object s, NotifyCollectionChangedEventArgs e) => method.Invoke(s, new object[] { s, e });

                    // field.SetValue(sender, orderedCollection);
                    /*
                    (Parent as Layout<View>).ForceLayout(); (( not making difference
                    for(int i = 0; i < (Parent as Layout<View>).Children.Count -1; i ++)
                    {
                        (Parent as Layout<View>).RaiseChild((Parent as Layout<View>).Children[i]);
                    }
                    */
                    /* won't be invoked?
                    var eventInfo = ObservableWrapperType.GetEvent("CollectionChanged");
                    eventInfo.GetRaiseMethod().Invoke(sender, new object[] { sender, eventArgs });
                    */
                    (Parent as Layout<View>).RaiseChild(this); //<--- holy shit so simple!!!

                });
            }
        }
        
        /*
                    Console.WriteLine("list became...");
                    foreach(var v in list)
                    {
                        Console.WriteLine(v);
                    }
                    */
        /*
        private void WhenCollectionChanged_Then(object sender, EventArgs e)
        {
            var list = sender as IList<View>;
            // Console.WriteLine("list was...");
            foreach (var v in list)
            {
                // Console.WriteLine(v);
            }
            var ordered = list.OrderBy((view) => (view is TopContentView)).ToList();
            if(!list.SequenceEqual(ordered))
            {
                // Console.WriteLine("ordered was...");
                foreach (var v in ordered)
                {
                    // Console.WriteLine(v);
                }
                for (int i = 0; i < ordered.Count; i++)
                {
                    if (ordered[i] != list[i])
                    {
                        var o = ordered[i];
                        var l = list[i];

                        // Console.WriteLine("replaced " + list[i] + " with " + ordered[i]);

                        list[i] = ordered[i];
                    }
                }

                // Console.WriteLine("list became...");
                foreach (var v in list)
                {
                    // Console.WriteLine(v);
                }

            }
        }
        */

        // AsyncEventListener whenCollectionChanged = new AsyncEventListener(); // static
        /* same obserbalecollection error as with collectiochanged below
        private void Layout_ChildAdded(object sender, ElementEventArgs e)
        {

        }
        */
        /*
        private void TopContentView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var n = e.NewItems;

            // https://stackoverflow.com/questions/21162881/use-linq-to-move-item-to-bottom-of-list
            (sender as INotifyCollectionChanged).CollectionChanged -= TopContentView_CollectionChanged;
            Device.BeginInvokeOnMainThread(() =>
            {
            var list = (Parent as Layout<View>).Children;
            // list[0] = new Label() { Text = "wohaowha" };

            if (list != null)
            {
                var ordered = list.OrderBy((view) => (view is TopContentView)).ToList();

                for (int i = 0; i < ordered.Count; i++)
                {

                    Console.WriteLine("i: " + i);
                    if (list[i] != ordered[i])
                    {
                        Console.WriteLine("replaced " + list[i] + " with " + ordered[i]);

                        list[i] = ordered[i];
                    }
                }

                // list.Add(new Label() { "oshdodhwodh" });


                }
            });
        }
        */

        /*
        private void Layout_ChildAdded(object sender, ElementEventArgs e)
        {
            Console.WriteLine(e.Element + " added");
        }
        */


        /*
        private void Layout_PropertyChanging(object sender, Xamarin.Forms.PropertyChangingEventArgs e)
        {
            var layout = sender as Layout<View>;
            Console.WriteLine(e.PropertyName + " will change");
            if(e.PropertyName == "Children")
            {
                Console.WriteLine("Children will change: " + layout.Children.Count);
            }
        }

        private void Layout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var layout = sender as Layout<View>;
            Console.WriteLine(e.PropertyName + " changed");
            if (e.PropertyName == "Children")
            {
                Console.WriteLine("Children changed: " + layout.Children.Count);
            }
        }
        */
        /* can run two times. This happens when verticaloptions/horizontaloptions is set on any control
    * inside the layout and app uses appshell/is shellcontent. (probably caused from navigationbar)
    * reproduce in new project, report bug.
    */
        private double width = -1;
        private double height = -1;
        private void TopContentView_LayoutChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Layout changed: " + X + ", " + Y + ", " + Width + ", " + Height + ". WidthRequest: " + WidthRequest + ". HeightRequest: " + HeightRequest);
            var layout = (sender as Layout<View>);

            Console.WriteLine("Rerender with new bounds");
            if (Width > 0)
            {
                width = Width;
            }
            if (Height > 0)
            {
                height = Height;
            }
            // flickering on topcontentview after adding a few labels -and sometimes dissappearing - has dissapeared
            // removes empty area created in the original position by the layout
            WidthRequest = 0;
            HeightRequest = 0;
            Console.WriteLine("width: " + width + ", height: " + height);
            Layout(new Rectangle(X, Y, width, height)); // makes the content

            var list = layout.Children;
            var ordered = list.OrderBy((view) => (view is TopContentView)).ToList();
            if (!list.SequenceEqual(ordered))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != ordered[i])
                    {
                        Device.InvokeOnMainThreadAsync(() =>
                        {
                            /* works
                            if(!list.Contains(label))
                            {
                                list[0] = label;
                            }
                            */
                            list[i] = null;

                        });
                    }
                }
            }

        }
        // https://forums.xamarin.com/discussion/158521/xaml-is-using-hidden-property#latest
        // https://stackoverflow.com/questions/56579689/xaml-using-hidden-property
        // can't use mobx beacuse xaml looks for BindableProperties first - hiding property declared in child
        // probably create an issue on xamarin forms about this

        // public static new BindableProperty XProperty = null;
        /*
        private static void XPropertyChanged(BindableObject bindable, object oldValue, object newValue )
        {
            Console.WriteLine("newValue: " + newValue);
        }
        */


        public static new BindableProperty XProperty =
            BindableProperty.Create(propertyName: "X",
                returnType: typeof(double), declaringType: typeof(TopContentView), propertyChanged: XPropertyChanged);

        public static void XPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            /* won't be set, probably 
            var property = typeof(VisualElement).GetProperty("X", BindingFlags.Instance | BindingFlags.Public);
            var set = property.GetSetMethod(true); // nonpublic: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo.getsetmethod?view=netframework-4.8
            set.Invoke(bindable, new object[] { newValue });
            */

            (bindable as TopContentView).X = (double)newValue;

        }
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
                var set = property.GetSetMethod(true); // nonpublic: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo.getsetmethod?view=netframework-4.8
                                                       // Console.WriteLine("setting: " + value);
                set.Invoke(this, new object[] { value });
            }
        }
        public static new BindableProperty YProperty =
            BindableProperty.Create(propertyName: "Y",
                returnType: typeof(double), declaringType: typeof(TopContentView), propertyChanged: YPropertyChanged);

        public static void YPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            /* won't be set, probably 
            var property = typeof(VisualElement).GetProperty("X", BindingFlags.Instance | BindingFlags.Public);
            var set = property.GetSetMethod(true); // nonpublic: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo.getsetmethod?view=netframework-4.8
            set.Invoke(bindable, new object[] { newValue });
            */

            (bindable as TopContentView).Y = (double)newValue;

        }

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
        public new static BindableProperty WidthRequestProperty =
            BindableProperty.Create("WidthRequest", typeof(double), typeof(TopContentView),
                propertyChanged: WidthRequestPropertyChanged);

        public static void WidthRequestPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TopContentView).WidthRequest = (double)newValue;
        }
        private double widthRequest;
        public new double WidthRequest
        {
            get { return widthRequest; }
            set
            {
                widthRequest = value;
            }
        }

        public new static BindableProperty HeightRequestProperty =
            BindableProperty.Create("HeightRequest", typeof(double), typeof(TopContentView),
                propertyChanged: HeightRequestPropertyChanged);

        public static void HeightRequestPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TopContentView).HeightRequest = (double)newValue;
        }
        private double heightRequest;
        public new double HeightRequest
        {
            get { return heightRequest; }
            set
            {

                heightRequest = value;
            }
        }
        */
    }

    // https://stackoverflow.com/questions/12858501/is-it-possible-to-await-an-event-instead-of-another-async-method by Anders Skovborg
    // used to get bounds after views are displayed - to set maxHeight with MaxItemsShown
    public class AsyncEventListener
    {

        Func<object, List<View>> func = (sender) =>
        {
            if (sender is IList<View> list) // is in fact ElementCollection
            {
                // var list = new List<View>(senderList);
                var ordered = list.OrderBy((view) => (view is TopContentView)).ToList();

                if (!list.SequenceEqual(ordered))
                {
                    // list[i] = new Label() { Text = "edited" }; // breaks out of action and not takes effect
                    Device.InvokeOnMainThreadAsync(() =>
                    {
                        /*
                        while(!list.SequenceEqual(list.OrderBy((view) => (view is TopContentView)).ToList()))
                        {
                            Console.WriteLine("ordered: ");
                            foreach(var v in )
                            
                        }
                        Console.WriteLine("became: ");
                        foreach(var v in list)
                        {
                            Console.WriteLine(v);
                        }
                        */
                        list[0] = new Label() { Text = "yoo" };
                    });

                }

                // Console.WriteLine("reordering children");

            }

            Console.WriteLine("completed sequence action");
            return null;
        };
        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
        List<Task> whileEvent = new List<Task>();
        int taskNumber = 1;
        public void Listen(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            NamedTask Successfully = new NamedTask("" + taskNumber, () =>
            {
                func.Invoke(sender);
            });
            Console.WriteLine("created " + Successfully.Name + " task");
            taskNumber++;
            Task.WhenAll(whileEvent).ContinueWith((completed) =>
            {

                whileEvent.Add(Successfully);

                Successfully.GetAwaiter().OnCompleted(() =>
                {
                    Console.WriteLine(Successfully.Name + " finished");
                    whileEvent.Remove(Successfully);
                });
                Console.WriteLine("started " + Successfully.Name);
                Successfully.Start();
            }).Wait();

        }

    }

    /**
     * For testing purpose to see and ensure that tasks run in sequence
     */
    public class NamedTask : Task
    {
        public string Name { get; set; }
        public NamedTask(string name, Action action) : base(action)
        {
            Name = name;
        }
    }
    public static class Extensions
    {
        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
        {
            Func<Type[], Type> getType;
            var isAction = methodInfo.ReturnType.Equals((typeof(void)));
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { methodInfo.ReturnType });
            }

            if (methodInfo.IsStatic)
            {
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
            }

            return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
        }
    }
    /*ignore
     * https://forums.xamarin.com/discussion/2840/running-build-from-command-line-fails jonathan pryor
     * https://github.com/mrxten/XamEffects
    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    Type ElementCollectionType = Type.GetType("Xamarin.Forms.ElementCollection`1,Xamarin.Forms.Core.dll", true); // can't access ObsevableWraer directly - not finding it in asssmbly: https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/ObservableWrapper.cs
    CollectionChanged = ElementCollectionType.BaseType.GetEvent("CollectionChanged");
    */

    /*
     * Need to hide WidthRequest and HeightRequest - so they can be set and used as expected
     * Need to correct/adjust for when parent layout has spacing
     * Might add functionality where selected topcontentview 
     * goes on top of other controls. (used if creating draggable ContentViews)
    */

}
