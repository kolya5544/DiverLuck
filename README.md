# DiverLuck
*DiverLuck* is objectively *the best* programming language ever done. It is an interpreted Turing-complete language allowing for creation of extremely complex and advanced software.

## Features

DiverLuck features next capabilities out of the box:
- If statements
- While blocks
- Function invokage
- OOP: accessing methods and fields of objects, creating instances of existing classes
- Function declaration and calls

## Compilation & running

Run `dotnet build` in folder with `DiverLuckInterpreter.sln`. Keep in mind, you will need *.NET 6* installed to compile and run this program. Upon successful compilation, you can access the interpreter by running `dotnet DiverLuckInterpreter.dll` in `\Interpreter\bin\Debug\net6.0` folder.

You can execute language commands there. Additionally, you can execute `webserver` command in the interpreter to execute a pre-built DiverLuck program (see `\Interpreter\Program.cs` file).

The shortest command you can run to check if DiverLuck works is `+)-)+).`, which will select and execute `WriteLine()` function.

## Language structure

In DiverLuck, almost every character is a command, which the exception of function calls and if statements. DiverLuck resembles Assembly and BrainFuck in numerous ways.

### Memory structure

DiverLuck memory consists of 1024 memory *cells*. Every memory cell contains a numerical value (zero by default) and an object (null by default). There is no limit on the type of an object stored inside a memory cell, as well as no limit on the size of the object stored. Numerical value, however, is `int` (signed 32-bit), and thus is limited to a range of any signed 32-bit integer.

Current memory cell selected is determined by *memory pointer*. By default, memory pointer is equal to 0, and thus points at the first memory cell.

Memory can be traversed and manipulated using commands:
- `<` allows you to decrement memory pointer, thus moving to a memory cell to your left.
- `>` increments memory pointer, moves you to a memory cell to your right.
- `+` increments current memory cell value by 1
- `-` decrements current memory cell value by 1
- `@` adds memory cell value to memory cell object (treated as integers, unless memory cell object is of other primitive type)
- `%` subtract memory cell value from memory cell object (same rule as above)
- `=` assigns memory cell *object* to current *value* (same rule as above)
- `~` assigns memory cell *value* to current *object* (attempts casting object to `int`, ensure that is possible)

For example, `+++` will increment current memory cell value by 3, and `+++=@` will first increment current memory cell value by 3, then create an `int` as memory cell object, and then add 3 to memory cell object (making current cell object be an `int` of value `6`)

### Stack structure

DiverLuck implements a global stack data structure. Stack can contain any objects of any types. DiverLuck stack is a typical stack and follows LIFO (Last In, First Out) structure.

Stack can be manipulated in several ways, most notably:
- `^` pushes *an object* contained within current memory cell to stack.
- `v` pulls *an object* from stack, and puts it in current memory cell as an object (overwriting current object, if any).

### Level structure

DiverLuck implements *level structure* allowing you to access different parts of *objects* contained within memory cells.

Level structure is an array of values a program can progress through to choose a *method* or *field*. 

It looks like this: `[NONE] -> Namespace -> Class -> Static Method -> Static Class Field/Property -> Instance Method -> Instance Field/Property`. Levels don't depend on memory cell currently chosen.

`LookupUtility` must be used to find IDs of necessary methods and fields.

Levels can be manipulated using two commands:
- `)` moves your executor one level up
- `(` moves your executor one level down

Example usage to choose a method like `WriteLine(String)` would be:
1. Use `LookupUtility` to find the method: `System.Console.Void WriteLine(System.String) -> namespace #11, class #52, method #82`
2. Traverse to the level necessary. First, traverse to the namespace necessary: `+++++++++++)`, then, to class necessary: `+++ <...> +++)`, then, to static method necessary in a similar way.
3. In case a property/field/method of a *current* object must be accessed, use appropriate levels.

On `Instance Field/Property` and `Static Class Field/Property` levels, **stack operations behave differently!** Instead of normal operations, these commands work like this:
- `^` pushes *the value* of a currently chosen field/property to the stack.
- `v` pulls an object from the stack *and sets it as current object field/property value*.

### Function invoking

DiverLuck programs can invoke .NET functions. Refer to *Level Structure* section.

Choose an appropriate level. Then:
- `.` is used to invoke a function.

Function return value is pushed to the stack. If function requires arguments, they are pulled from the stack, too, so make sure to push all necessary values to the stack!

### User functions

DiverLuck programs can define their own functions. For that:
- a user-defined function must begin with `&` 
- a user-defined function must end with `\`

Keep in mind stack values and memory cell states might be changed by functions, thus it is the duty of the developer to ensure functions are safe to call before calling them.

Functions can be called using `#N$`, where N is the ID of function. First function declared has an ID of `0`, so `#0$` shall be used to call the function.

### "If" conditions

DiverLuck allows program to implement if conditions. For that:
- `i` shall mark the beginning of any if condition
- a character must follow immediately, which is one of these, depending on condition type: `e` for "equal", `n` for "not equal", `m` for "more than" and `l` for "less than"
- a number must follow the character, which is the value compared against. The number shall not exceed the value range of a signed 32-bit integer
- `f` shall mark the end of the if condition
- `{` should follow the condition immediately, marking the beginning of code block that will only be executed if the condition is true
- `}` marks the end of the code block.

For example, `ie5f{.}` will only invoke a function if the value of current cell is equal to `5`, and `in0f{)}` will only change current level if the value of current cell is not equal to `0`

### "While" loops

DiverLuck allows programs to implement endless loops. For that:
- while loop shall begin with `[`
- while loop shall end with `]`

A loop will be executed endlessly upon reach. Termination of a loop is possible in two ways:
- while loop iteration can be skipped to the beginning using `c` (continue)
- while loop can be escaped using `b` (break), in which case execution continues from where the program left off (usually right after `]`, the end of while loop)

While loops can be used to set values of cells: `[ie10f{b}+]` will set the value of current cell to 10 if its initial value is below 11.
They can also be used to prevent the program from halting execution, as well as for complex math operations, like integer multiplication or division.

### Reserved core commands

DiverLuck implements several additional reserved commands that allow for easier manipulation of memory cell data. Those include:
- `0` creates an object of primitive type and puts it into current memory cell object. Primitive type created is determined by memory cell value. Primitive types that can be created this way, and their appropriate memory cell values, are: `(0) bool, (1) byte, (2) sbyte, (3) short, (4) ushort, (5) int, (6) uint, (7) long, (8) ulong, (9) IntPtr, (10) UIntPtr, (11) char, (12) double, (13) float`.
- `1` creates a list of Type of memory cell object. For example, if memory cell object is of type `string`, executing this command will create `List<string>`. However, if current memory cell object is `null`, a List<> of primitive type will be created depending on current memory cell value (see the primitive types above).
- `2` accesses list contained within current memory cell (as an object) by index, where index is current memory cell value. Value acquired is pushed to the stack.
- `3` pulls an object from stack and pushes it to the list contained within current memory cell
- `4` removes an object from the list contained within current memory cell by index, where index is current memory cell value.
- `5` creates an instance of currently selected class (see *Level Structure*). Constructor used is determined by current stack contents - push appropriate objects of appropriate types to influence the choice of constructor.

### Example programs

[Simple webserver running at :8183, with comments](https://github.com/kolya5544/DiverLuck/blob/master/Interpreter/Program.cs#L27L112)

## Contribution

No.

There is no reason this language should exist. It is, however, slightly admirable how such basic features and such primitive control still allows you to build virtually any type of software you can build in C#.

I've built the entire language specification and interpreter code alone in a single day, and I don't want that to change.