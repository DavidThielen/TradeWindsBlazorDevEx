<img align="left" width="200" src="element_into_input.png"/>

# TradeWindsBlazorDevEx

These are some components I created for my Blazor Interactive Server app using DevExpress components.

DevEx is free to add these, with any edits, to their library with no payment to me. I gift them this code.

> This is under the MIT license. If you find this very useful I ask (not a requirement) that you consider reading my book [I DON’T KNOW WHAT I’M DOING!: How a Programmer Became a Successful Startup CEO](https://a.co/d/bEpDlJR).
> 
> And if you like it, please review it on Amazon and/or GoodReads. The number of legitimate reviews helps a lot. Much appreciated.

## PopupMessageBox

This provides the simple WinForms MessageBox, both an async and a synchronous one. Critical to using this (see the SampleMainLayout.razor) is in MainLayout you must have:

```xml
<PopupMessageBox>
	<main> @Body </main>
</PopupMessageBox>
```
 And then in your components that use it (see the SampleCounter.razor):
 
 ```csharp
[CascadingParameter] 
protected PopupMessageBox PopupMessageBox { get; set; } = default!;
```

Do not access this via `[Inject]` as that will be null for some of the first events Blazor calls on your component. Using `[CascadingParameter]` insures the `PopupMessageBox` variable is populated before any events are called.


