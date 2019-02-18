using System;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Microsoft.CSharp;
using System.Text;
using Xunit.Abstractions;

namespace JsonUnFlat.Tests
{
    public class FlatTests
    {

         private  readonly ITestOutputHelper _output;

        public FlatTests(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void Test_one_nesting_level_objec_flattening_success()
        {
            var testObjString = "{\"prop1\":\"val1\",\"prop2\":{\"prop3\":123}}";
            var expectedResultString = "{\"prop1\":\"val1\",\"prop2.prop3\":123}";
            runTest(testObjString, expectedResultString);
        }

        [Fact]
        public void Test_one_nesting_level_object_several_props_flattening_success()
        {
            var testObjString = "{\"prop2\":{\"prop3\":123, \"prop4\": 345}}";
            var expectedResultString = "{\"prop2.prop3\":123, \"prop2.prop4\": 345}";

            runTest(testObjString, expectedResultString);
        }

        [Fact]
        public void Test_two_nesting_level_objec_flattening_success()
        {
            var testObjString = "{\"prop1\":123,\"prop2\":{\"prop3\":{\"prop4\":\"asd\"}}}";
            var expectedResultString = "{\"prop1\":123,\"prop2.prop3.prop4\":\"asd\"}";

            runTest(testObjString, expectedResultString);
        }

        [Fact]
        public void Test_array_object_flattening_success()
        {
            var testObjString = "{\"prop2\":[{\"prop3\":32,\"prop4\":\"asd\"}]}";
            var expectedResultString = "{\"prop2[0].prop3\":32, \"prop2[0].prop4\":\"asd\"}";

            runTest(testObjString, expectedResultString);
        }

        [Fact]
        public void Test_json_with_primitive_array_flattening_success()
        {
            var testObjString = "{\"p1\":[123,234],\"p2\":235,\"p3\":{\"p4\":567}}";
            var expectedResultString = "{\"p1[0]\":123,\"p1[1]\":234,\"p2\":235,\"p3.p4\":567}";
            runTest(testObjString, expectedResultString);
        }

        private void runTest(string testJson, string expectedJson)
        {
            var flattener = new JsonFlatter();
            var testObj = JObject.Parse(testJson);
            _output.WriteLine("TEST DATA: " + testObj);

            var result = new JObject();

            flattener.Flat("", testObj, ref result);
            _output.WriteLine("RESULT: " + result);

            var expectedResult = JObject.Parse(expectedJson);
            _output.WriteLine("EXPECTED RESULT: " + expectedResult);
            //result.Should().Equal(expectedResult);
            JToken.DeepEquals(result, expectedResult).Should().BeTrue();
        }


    }
}
