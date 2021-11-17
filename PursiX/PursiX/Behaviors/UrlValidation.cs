using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace PursiX.Behaviors
{
    class UrlValidation : Behavior<Entry>
        {
        //https://www.c-sharpcorner.com/article/input-validation-in-xamarin-forms-behaviors/

        const string urlRegex = @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$";


            protected override void OnAttachedTo(Entry bindable)
            {
                bindable.TextChanged += HandleTextChanged;
                base.OnAttachedTo(bindable);
            }

            void HandleTextChanged(object sender, TextChangedEventArgs e)
            {
                bool IsValid = false;
                IsValid = (Regex.IsMatch(e.NewTextValue, urlRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
                ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
            }

            protected override void OnDetachingFrom(Entry bindable)
            {
                bindable.TextChanged -= HandleTextChanged;
                base.OnDetachingFrom(bindable);
            }
    }


    
}
