# ObjectCartographer

[![.NET Publish](https://github.com/JaCraig/ObjectCartographer/actions/workflows/dotnet-publish.yml/badge.svg)](https://github.com/JaCraig/ObjectCartographer/actions/workflows/dotnet-publish.yml)

ObjectCartographer is a fast, convention based, and developer friendly object to object mapper. It's designed to simplify your life and remove the drudgery of writing code to copy data from one object to another.

## Setting Up the Library

ObjectCartographer uses a library called Canister for registering itself in your ServiceCollection:

    servicecollection.AddCanisterModules();

With that ObjectCartographer will automatically register any converters found in your application and work with your DI system if you are using one, allowing you to access the DataMapper object at run time if you need to. Otherwise if you are not using one, you can simply use the extension methods and it will wire itself up.

## Basic Usage

Once the initial setup is done, we need to map our objects to each other. This is accomplished in a number of different ways. First by using the DataMapper class:

    DataMapper.Map<MyClass1, MyClass2>()
                .AddMapping(MyClass1 => MyClass1.PropertyToReadFrom, (MyClass2, value) => MyClass2.PropertyToWriteTo = value)
                .AddMapping(MyClass1 => MyClass1.Property1 + MyClass1.Property2, (MyClass1, value) => MyClass2.ComputedProperty = value)
                .AddMapping(MyClass1 => MyClass1.A.B.C.D, (MyClass1, value) => MyClass2.ProjectionProperty = value)
                .Build();

The above code could also be written using the extension method Map:

    MyClass1Object.Map<MyClass1, MyClass2>()
                    .AddMapping(MyClass1 => MyClass1.PropertyToReadFrom, (MyClass2, value) => MyClass2.PropertyToWriteTo = value)
                    .AddMapping(MyClass1 => MyClass1.Property1 + MyClass1.Property2, (MyClass1, value) => MyClass2.ComputedProperty = value)
                    .AddMapping(MyClass1 => MyClass1.A.B.C.D, (MyClass1, value) => MyClass2.ProjectionProperty = value)
                    .Build();
					
You can also supply your own method for copying the data:

    MyClass1Object.Map<MyClass1, MyClass2>()
                    .UseMethod(MyCopier)
                    .Build();

It's also possible, if you'd prefer, for the system to map everything for you based on the conventions that the system uses:

    DataMapper.AutoMap<MyClass1, MyClass2>();

Or:

    MyClass1Object.AutoMap<MyClass2>();

And lastly, you can simply skip the above steps all together and simply start using the library:

    MyClass2 Result = MyClass1Object.To<MyClass2>();

If you don't set up the mapping beforehand, the library will go through the properties on MyClass1 and map them to properties with the same name on MyClass2. It compares the property names by first looking for exact matches, then it will drop underscores and compare them ignoring case.

## Give Me Speed

The library is about 34% faster than AutoMapper in more complex scenarios:

``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.18363.1440 (1909/November2019Update/19H2)
Intel Core i7-9850H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.203
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  DefaultJob : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT


```
|                            Method |      Mean |     Error |    StdDev | Ratio | RatioSD | Rank |
|---------------------------------- |----------:|----------:|----------:|------:|--------:|-----:|
|                        AutoMapper | 91.575 ns | 0.9259 ns | 0.8661 ns |  1.34 |    0.02 |    2 |
|                ObjectCartographer | 68.374 ns | 0.6952 ns | 0.6163 ns |  1.00 |    0.00 |    1 |

And about 350% faster when an object is supplied to it and only data copying is required.

``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.18363.1440 (1909/November2019Update/19H2)
Intel Core i7-9850H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.203
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  DefaultJob : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT


```
|             Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD | Rank |
|------------------- |----------:|----------:|----------:|----------:|------:|--------:|-----:|
|         AutoMapper | 88.796 ns | 1.4598 ns | 3.2951 ns | 87.404 ns |  3.48 |    0.16 |    2 |
| ObjectCartographer | 26.804 ns | 0.2989 ns | 0.2650 ns | 26.712 ns |  1.00 |    0.00 |    1 |

## Installation

The library is available via Nuget with the package name "ObjectCartographer". To install it run the following command in the Package Manager Console:

Install-Package ObjectCartographer

Note that there is a package that adds mapping for ADO.Net specific data types: "ObjectCartographer.SQL" so the system can do things like map DbType to SqlDbType along with other functionality.

## FAQ

1. How do I add my own converter to the system?

You would need to implement the ObjectCartographer.ExpressionBuilder.Interfaces.IConverter interface. There is also the ObjectCartographer.ExpressionBuilder.BaseClasses.ConverterBaseClass abstract class to help with destination object creation/copy constructor discovery which is good in instances where you are mapping more complex objects. For simple data conversions like string to an int, the IConverter interface should be enough.

## Build Process

In order to build the library you may require the following:

1. Visual Studio 2022

Other than that, just clone the project and you should be able to load the solution and build without too much effort.
