using System;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Microsoft.CSharp;
using System.Text;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace JsonUnFlat.Tests
{
    public class UnflatTests
    {
        private  readonly ITestOutputHelper _output;

        public UnflatTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test_one_nesting_level_json_unflattened()
        {
            string testFlatJson = "{\"prop1.prop2\":123,\"prop3.prop4\":343}";
            string expectedUnflattenedJson = "{\"prop1\":{\"prop2\":123},\"prop3\":{\"prop4\":343}}";
            runTest(testFlatJson, expectedUnflattenedJson);
        }

        [Fact]
        public void Test_plain_json_unflattened()
        {
            string testFlatJson = "{\"prop1\":123,\"prop3\":343}";
            string expectedUnflattenedJson = "{\"prop1\":123,\"prop3\":343}";
            runTest(testFlatJson, expectedUnflattenedJson);
        }

        [Fact]
        public void Test_array_json_unflattened()
        {
            var testFlatJson = "{\"prop2[0].prop3\":32, \"prop2[0].prop4\":\"asd\"}";
            var expectedUnflattenedJson = "{\"prop2\":[{\"prop3\":32,\"prop4\":\"asd\"}]}";
            runTest(testFlatJson, expectedUnflattenedJson);
        }

        [Fact]
        public void Test_array_nested_object_combined_json_unflattened()
        {
            var testFlatJson = "{\"prop1[0].prop2\":123,\"prop1[1].prop3\":434,\"prop4.prop5.prop6[0].prop7\":123}";
            var expectedUnflattenedJson = "{\"prop1\":[{\"prop2\":123},{\"prop3\":434}],\"prop4\":{\"prop5\":{\"prop6\":[{\"prop7\":123}]}}}";
            runTest(testFlatJson, expectedUnflattenedJson);
        }

        [Fact]
        public void Test_array_multipleindexes_json_unflattened()
        {
            var expectedUnflattenedJson = "{\"prop1\":[{\"prop2\":123},{\"prop3\":234}]}";
            var testFlatJson = "{\"prop1[0].prop2\":123,\"prop1[1].prop3\":234}";
            runTest(testFlatJson, expectedUnflattenedJson);
        }


        private void runTest(string testJson, string expectedResultJson)
        {
            var testObj = JObject.Parse(testJson);
            _output.WriteLine("TEST DATA: " + testObj);
            var unflattener = new JsonFlatter();

            var result = unflattener.Unflat(testObj);
            _output.WriteLine("RESULT: " + result);
            var expectedResult = JObject.Parse(expectedResultJson);
            _output.WriteLine("EXPECTED RESULT: " + expectedResult);
            JToken.DeepEquals(result, expectedResult).Should().BeTrue();
        }
    }
}