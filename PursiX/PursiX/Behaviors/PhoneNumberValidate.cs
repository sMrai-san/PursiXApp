using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace PursiX.Behaviors
{
    class PhoneNumberValidate : Behavior<Entry>
        {
        //https://www.c-sharpcorner.com/article/input-validation-in-xamarin-forms-behaviors/

        const string phoneRegex = @"^((([\+][\s]{0,1})|([0]{2}[\s-]{0,1}))([358]{3})([\s-]{0,1})|([0]{1}))(([1-9]{1}[0-9]{0,1})([\s-]{0,1})([0-9]{2,4})([\s-]{0,1})([0-9]{2,4})([\s-]{0,1}))([0-9]{0,3}){1}$";


            protected override void OnAttachedTo(Entry bindable)
            {
                bindable.TextChanged += HandleTextChanged;
                base.OnAttachedTo(bindable);
            }

            void HandleTextChanged(object sender, TextChangedEventArgs e)
            {
                bool IsValid = false;
                IsValid = (Regex.IsMatch(e.NewTextValue, phoneRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
                ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
            }

            protected override void OnDetachingFrom(Entry bindable)
            {
                bindable.TextChanged -= HandleTextChanged;
                base.OnDetachingFrom(bindable);
            }
    }


    
}
