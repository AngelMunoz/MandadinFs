namespace Library.Todos

open System
open System.Reactive.Subjects

open Avalonia
open Avalonia.Data
open Avalonia.Controls

open NXUI.FSharp.Extensions
open Avalonia.Controls.Templates
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Media

open Library.Services.Todos

module UI =

  let TodoForm onAddTodo =
    let textbox =
      TextBox()
        .acceptsReturn(true)
        .maxWidth(300.)
        .watermark("Todo Title")
        .useFloatingWatermark(true)
        .OnKeyUpHandler(fun t e ->
          e.Handled <- true

          match e.Key with
          | Key.Enter when
            e.KeyModifiers = KeyModifiers.Control
            && not(String.IsNullOrWhiteSpace t.Text)
            ->
            onAddTodo(t.Text)
            t.Text <- ""
          | _ -> ()
        )

    let enableAdd =
      textbox.ObserveText()
      |> Observable.map(fun text -> not(String.IsNullOrWhiteSpace text))

    let submitButton =
      Button()
        .DockRight()
        .isEnabled(enableAdd.ToBinding(), mode = BindingMode.OneWay)
        .content("Add")
        .OnClickHandler(fun _ _ ->
          onAddTodo(textbox.Text)
          textbox.Text <- ""
        )

    DockPanel()
      .lastChildFill(true)
      .VerticalAlignmentCenter()
      .HorizontalAlignmentStretch()
      .children(
        submitButton
          .margin(4.)
          .VerticalAlignmentCenter(),
        textbox
          .margin(4.)
          .VerticalAlignmentCenter()
      )

  let inline TodoItem todo onTodoChecked onTodoDeleted : Control =
    let toggle =
      CheckBox()
        .DockLeft()
        .isChecked(todo.Completed)
        .OnIsCheckedChangedHandler(fun _ _ -> onTodoChecked todo)

    let title =
      TextBlock()
        .text(todo.Title)
        .fontSize(16.)
        .textDecorations(
          if todo.Completed then
            TextDecorations.Strikethrough
          else
            null
        )

    let delete =
      Button()
        .DockRight()
        .content("Delete")
        .OnClickHandler(fun _ _ -> onTodoDeleted todo)

    DockPanel()
      .lastChildFill(true)
      .HorizontalAlignmentStretch()
      .VerticalAlignmentStretch()
      .children(
        toggle.VerticalContentAlignmentCenter(),
        delete.VerticalContentAlignmentCenter(),
        title.VerticalAlignmentCenter()
      )


  let TodosPage (todos: #TodoService) : Control =

    DockPanel()
      .lastChildFill(true)
      .verticalAlignment(VerticalAlignment.Stretch)
      .horizontalAlignment(HorizontalAlignment.Stretch)
      .children(
        Label()
          .DockTop()
          .content("Todos")
          .fontSize(24.)
          .fontWeight(FontWeight.Bold)
          .horizontalAlignment(HorizontalAlignment.Center),
        Expander()
          .DockBottom()
          .HorizontalAlignmentStretch()
          .VerticalAlignmentBottom()
          .expandDirection(ExpandDirection.Up)
          .header(Label().content("Add Todo"))
          .content(TodoForm todos.AddNew),
        ScrollViewer()
          .content(
            ItemsControl()
              .itemsSource(todos.Todos.ToBinding(), mode = BindingMode.OneWay)
              .itemTemplate(
                new FuncDataTemplate<Todo>(
                  (fun todo _ ->
                    TodoItem
                      todo
                      (todos.Toggle >> ignore)
                      (todos.Delete >> ignore)
                  ),
                  true
                )
              )
          )
      )
