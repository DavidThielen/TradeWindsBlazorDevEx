
// Copyright (c) 2024 Trade Winds Studios (David Thielen)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace TradeWindsBlazorDevEx.Components
{
	/// <summary>
	/// A MessageBox (async and synchronized) component to provide a way to show informational messages.
	/// This does <b>not</b> inherit ExComponentBase because it's an outer cascading parameter and all those
	/// injections may not be set yet.
	/// </summary>
	public partial class PopupMessageBox : ComponentBase
	{

		public static readonly string ButtonOk = "Ok";
		public static readonly string ButtonCancel = "Cancel";
		public static readonly string ButtonYes = "Yes";
		public static readonly string ButtonNo = "No";

		[Inject]
		protected NavigationManager Navigation { get; set; } = default!;

		/// <summary>
		/// This component contains the main content in this child content. By wrapping the main content
		/// it insures the cascading parameter is always set from the get-go in the main content.
		/// </summary>
		[Parameter] public RenderFragment? ChildContent { get; set; }

		/// <summary>
		/// The MessageBox popup. This is a DxPopup that is called asynchronously so it is displayed and control returns
		/// to the caller.
		/// </summary>
		private DxPopup MessageBoxPopup { get; set; } = default!;

		/// <summary>
		/// The header text in the MessageBox popup. This is a MarkupString so do not put any user entered text in this!
		/// </summary>
		private string HeaderText { get; set; }

		/// <summary>
		/// The message text in the MessageBox popup. This is a MarkupString so do not put any user entered text in this!
		/// </summary>
		private string MessageHtml { get; set; }

		private Func<string, object?, Task>? OnClick { get; set; }

		private Func<object?, Task>? OnClose { get; set; }

		/// <summary>
		/// This is passed in in the call to Show() and returned in the calls to OnClick() and OnClose().
		/// </summary>
		private object? Tag { get; set; }

		/// <summary>
		/// Used to make the popup synchronous.
		/// </summary>
		private TaskCompletionSource<string> _taskCompletionSource;

		public PopupMessageBox()
		{
			_taskCompletionSource = new TaskCompletionSource<string>();
			ButtonVisible = new bool[4];
			ButtonText = new string[4];
			HeaderText = string.Empty;
			MessageHtml = string.Empty;
		}

        /// <summary>
        /// Displays the MessageBox. This is a static object so you can't create multiple instances of it. But you
        /// can create another instance in the onClick or onClose calls as the existing instance will be closed by
        /// then.
        /// </summary>
        /// <param name="header">The header text in the message box.</param>
        /// <param name="message">The main message in the message box. This is treated as html
        ///     so use <code>HttpUtility.HtmlEncode()</code> on any user generated text in the message.</param>
        /// <param name="tag">Caller defined data returned in the OnClick/OnClose calls.</param>
        /// <param name="onClick">Will call this method when a button is clicked. Passes the button text as the parameter.</param>
        /// <param name="onClose">Will call this when the popup is closed. Including after receiving the onClick call.</param>
        /// <param name="buttonText">The text for the button(s). Can be 1 .. 4 buttons.</param>
        /// <returns>The task for the underlying DxPopup.ShowAsync() call.</returns>
        public Task<bool> ShowAsync(string header, string message, object? tag, Func<string, object?, Task>? onClick,
            Func<object?, Task>? onClose, params string[] buttonText)
		{

			HeaderText = header;
			MessageHtml = message;
            Tag = tag;
			OnClick = onClick;
			OnClose = onClose;

			if (buttonText.Length == 0 || buttonText.Length > 4)
				throw new ArgumentOutOfRangeException(nameof(buttonText), "Must have between 1 and 4 buttons");
			Array.Clear(ButtonVisible);
			Array.Clear(ButtonText);
			for (var index = 0; index < buttonText.Length; index++)
			{
				ButtonVisible[index] = true;
				ButtonText[index] = buttonText[index];
			}

			StateHasChanged();

			return MessageBoxPopup.ShowAsync();
		}

		/// <summary>
		/// Displays the MessageBox. Then when click OK, goes to the url. This is a static object so you can't create
		/// multiple instances of it. 
		/// </summary>
		/// <param name="header">The header text in the message box.</param>
		/// <param name="message">The main message in the message box. This is treated as html
		/// so use <code>HttpUtility.HtmlEncode()</code> on any user generated text in the message.</param>
		/// <param name="url">The url to navigate to when click OK.</param>
		/// <returns>The task for the underlying DxPopup.ShowAsync() call.</returns>
		public Task<bool> ShowThenRedirectAsync(string header, string message, string url)
		{
			return ShowAsync(header, message, null,
				(_, _) =>
                {
                    Navigation.NavigateTo(url, ForceReload(url));
                    return Task.CompletedTask;
                },
				(_) =>
				{
					Navigation.NavigateTo(url, ForceReload(url));
					return Task.CompletedTask;
				},
				ButtonOk);
		}

		/// <summary>
		/// Displays the MessageBox <b>synchronously</b>. Returns the text of the button clicked. This is a static object
		/// so you can't create multiple instances of it. But you can create another instance when it returns as the
		/// existing instance will be closed by then.
		/// </summary>
		/// <param name="header">The header text in the message box.</param>
		/// <param name="message">The main message in the message box. This is treated as html
		/// so use <code>HttpUtility.HtmlEncode()</code> on any user generated text in the message.</param>
		/// <param name="buttonText">The text for the button(s). Can be 1 .. 4 buttons.</param>
		/// <returns>The text of the button clicked.</returns>
		public Task<string> Show(string header, string message, params string[] buttonText)
		{
			_taskCompletionSource = new TaskCompletionSource<string>();

			ShowAsync(header, message, null, ShowOnClick, null, buttonText);

			// we return the Task from the TaskCompletionSource that is not completed
			return _taskCompletionSource.Task;
		}

		private Task ShowOnClick(string buttonText, object? tag)
		{
			// as this is called from the OnClick handler, the popup has been closed.

			// sets the TaskCompletionSource to completed.  Any await-ers will now complete
			_taskCompletionSource.SetResult(buttonText);
			return Task.FromResult(_taskCompletionSource);
		}

		private string[] ButtonText { get; }

		private bool[] ButtonVisible { get; }

		private async Task Button0Click(MouseEventArgs arg)
		{
			// need to close before calling OnClick because OnClick may call ShowAsync again
			await MessageBoxPopup.CloseAsync();
			if (OnClick != null)
				await OnClick.Invoke(ButtonText[0], Tag);
		}

		private async Task Button1Click(MouseEventArgs arg)
		{
			// need to close before calling OnClick because OnClick may call ShowAsync again
			await MessageBoxPopup.CloseAsync();
			if (OnClick != null)
				await OnClick.Invoke(ButtonText[1], Tag);
		}
		private async Task Button2Click(MouseEventArgs arg)
		{
			// need to close before calling OnClick because OnClick may call ShowAsync again
			await MessageBoxPopup.CloseAsync();
			if (OnClick != null)
				await OnClick.Invoke(ButtonText[2], Tag);
		}
		private async Task Button3Click(MouseEventArgs arg)
		{
			// need to close before calling OnClick because OnClick may call ShowAsync again
			await MessageBoxPopup.CloseAsync();
			if (OnClick != null)
				await OnClick.Invoke(ButtonText[3], Tag);
		}

		private async Task PopupClosed(PopupClosedEventArgs arg)
		{
			// need to close before calling OnClose because OnClose may call ShowAsync again
			await MessageBoxPopup.CloseAsync();
			if (OnClose != null)
				await OnClose.Invoke(Tag);
		}

		/// <summary>
		/// true if need to do a force reload because going to the identity pages. They're MVC and don't
		/// work with Blazor SPA.
		/// </summary>
		/// <param name="url">The url to parse for starting with Identity.</param>
		private static bool ForceReload(string? url)
		{
			if (string.IsNullOrEmpty(url))
				return false;
			var urlUpper = url.ToUpper();
			return urlUpper.StartsWith("/IDENTITY") || urlUpper.StartsWith("IDENTITY");
		}
	}
}
