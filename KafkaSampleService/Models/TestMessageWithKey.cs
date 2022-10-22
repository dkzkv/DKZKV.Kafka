// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618
namespace KafkaSampleService.Models;

public class TestMessageWithKey : TestMessage
{
    public string KeyPartOne { get; set; }
    public string KeyPartTwo { get; set; }
}