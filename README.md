# Challenge FSharp Starter

A starter template for F# challenges.

Using this starter kit will allow you to organize your code in multiple source files and generate a single file for challenge web sites like [CodinGame](https://www.codingame.com). This template also include a unit test project based on [xUnit.net](https://xunit.github.io/).

## Installation

* Download and install the [.NET SDK](https://aka.ms/dotnetcoregs).
* Clone this repo.

## Usage

This project uses [FAKE](https://fake.build/) to create and execute build targets. The two shell scripts `fake.sh` and `fake.cmd` can be used to bootstrap and run FAKE.

### Clean and build all

```
./fake.sh build -t All
```

Clean all generated files, build projects, run unit tests and merge the source files into the output file `./build/Challenge.fs`.

### Watch and merge files

```
./fake.sh build
```

Merge the source files in watch mode. This will regenerate the output file `./build/Challenge.fs` every time a source file will be saved.

> Press `Enter` to exit the watch mode in a clean way.

### List FAKE targets

```
./fake.sh build --list
```

List all available FAKE targets: `Clean, Build, BuildTests, Test, Merge, All, Watch`.