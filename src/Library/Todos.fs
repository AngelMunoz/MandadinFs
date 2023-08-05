namespace Library.Todos

open System
open System.Reactive.Subjects

open Avalonia
open Avalonia.Data
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent

open NXUI.FSharp.Extensions


module UI =
  open Library.Services.Todos
  open Avalonia.Controls.Templates
  open Avalonia.Layout
  open Avalonia.Media
  open Avalonia.Input

  let TodoForm onAddTodo =
    let checkbox =
      CheckBox()
        .content(Label().content("Is Completed"))

    let textbox =
      TextBox()
        .acceptsReturn(false)
        .watermark("Todo Title")
        .OnKeyUpHandler(fun t e ->
          match e.Key with
          | Key.Enter when
            e.KeyModifiers = KeyModifiers.Control
            && not(String.IsNullOrWhiteSpace t.Text)
            ->
            onAddTodo(t.Text, checkbox.IsChecked)
          | _ -> ()
        )

    let enableAdd =
      textbox.ObserveText()
      |> Observable.map(fun text -> not(String.IsNullOrWhiteSpace text))

    StackPanel()
      .orientation(Orientation.Horizontal)
      .spacing(4.)
      .children(
        textbox,
        checkbox,
        Button()
          .isEnabled(enableAdd.ToBinding(), mode = BindingMode.OneWay)
          .content("Add")
          .OnClickHandler(fun _ _ ->
            onAddTodo(textbox.Text, checkbox.IsChecked)
          )
      )

  let inline TodoItem todo onTodoChecked onTodoDeleted : Control =
    let toggle =
      CheckBox()
        .isChecked(todo.Completed)
        .OnIsCheckedChangedHandler(fun _ _ -> onTodoChecked todo)

    let title =
      TextBlock()
        .text(todo.Title)
        .textDecorations(
          if todo.Completed then
            TextDecorations.Strikethrough
          else
            null
        )

    let delete =
      Button()
        .content("Delete")
        .OnClickHandler(fun _ _ -> onTodoDeleted todo)

    StackPanel()
      .orientation(Orientation.Horizontal)
      .spacing(4.)
      .children(toggle, title, delete)


  let TodosPage (todos: #TodoService) : Control =

    let todoItem =
      new FuncDataTemplate<Todo>(
        (fun todo _ ->
          TodoItem todo (todos.Toggle >> ignore) (todos.Delete >> ignore)
        ),
        false
      )

    let onAddTodo (title, isCompleted) =
      let isCompleted = defaultArg (Option.ofNullable isCompleted) false
      todos.AddNew(title, isCompleted) |> ignore

    DockPanel()
      .children(
        ItemsControl()
          .DockTop()
          .HorizontalAlignmentStretch()
          .VerticalAlignmentStretch()
          .itemsSource(todos.Todos.ToBinding(), mode = BindingMode.OneWay)
          .itemTemplate(todoItem),
        Expander()
          .DockBottom()
          .HorizontalAlignmentStretch()
          .VerticalContentAlignmentCenter()
          .expandDirection(ExpandDirection.Up)
          .header(Label().content("Add Todo"))
          .content(TodoForm onAddTodo)
      )
