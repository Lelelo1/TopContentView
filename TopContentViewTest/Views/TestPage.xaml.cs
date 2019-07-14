using System;
using System.Collections.Generic;
using Control;
using Xamarin.Forms;

namespace TopContentViewTest.Views
{
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();
        }
        public void Handle_Clicked(object sender, EventArgs eventArgs)
        {
            /*
            Console.WriteLine("pre X: " + topContentView.X);
            var t = topContentView;
            // 597?
            
            var ch = stack.Children;
            Console.WriteLine("topContentView X: " + topContentView.X + " Y: " + topContentView.Y);
            topContentView.WidthRequest = -1;
            topContentView.HeightRequest = -1;
            */

            Console.WriteLine("width iz: " + this.Width + "height " + this.Height);
            /*
            var x = topContentView.X;
            var y = topContentView.Y;
            stack.Children.Remove(topContentView);
            stack.Children.Add(topContentView);
            topContentView.X = x;
            topContentView.Y = y;
            */
            // topContentView.X = topContentView.X;
            // topContentView.Y = 508;
            stack.WidthRequest = 160;
            stack.HeightRequest = 200;
        }
    }
}
