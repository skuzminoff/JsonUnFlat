using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace JsonUnFlat.Tests
{
    public class FlatTests
    {

        private readonly ITestOutputHelper _output;

        public FlatTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test_one_nesting_level_object_flat_successfull()
        {
            var test = "{\"prop1\":\"val1\",\"prop2\":{\"prop3\":123}}";
            var expectedResult = "{\"prop1\":\"val1\",\"prop2.prop3\":123}";
            runTest(test, expectedResult);
        }

        [Fact]
        public void Test_one_nesting_level_object_several_props_flat_successfull()
        {
            var test = "{\"prop2\":{\"prop3\":123, \"prop4\": 345}}";
            var expectedResult = "{\"prop2.prop3\":123, \"prop2.prop4\": 345}";

            runTest(test, expectedResult);
        }

        [Fact]
        public void Test_two_nesting_level_objec_flat_successfull()
        {
            var test = "{\"prop1\":123,\"prop2\":{\"prop3\":{\"prop4\":\"asd\"}}}";
            var expectedResult = "{\"prop1\":123,\"prop2.prop3.prop4\":\"asd\"}";

            runTest(test, expectedResult);
        }

        [Fact]
        public void Test_array_object_flat_successfull()
        {
            var test = "{\"prop2\":[{\"prop3\":32,\"prop4\":\"asd\"}]}";
            var expectedResult = "{\"prop2[0].prop3\":32, \"prop2[0].prop4\":\"asd\"}";

            runTest(test, expectedResult);
        }

        [Fact]
        public void Test_json_with_primitive_array_flat_success()
        {
            var test = "{\"p1\":[123,234],\"p2\":235,\"p3\":{\"p4\":567}}";
            var expectedResult = "{\"p1[0]\":123,\"p1[1]\":234,\"p2\":235,\"p3.p4\":567}";
            runTest(test, expectedResult);
        }

        [Fact]
        public void Test_json_has_datetime_prop_flat_successfull()
        {
            var test = "{\"prop1\":{\"prop2\":\"2017-09-08T19:04:14.480Z\"},\"prop3\":{\"prop4\":\"2017-09-08\"},\"prop5\":[\"2017-09-03\",\"2017-09-04\",\"2017-09-05\"],\"prop6\":[\"2017-09-08T19:04:14.480Z\",\"2017-09-03T19:04:14.480Z\"]}";
            var expectedResult = "{\"prop1.prop2\":\"2017-09-08T19:04:14.480Z\",\"prop3.prop4\":\"2017-09-08\",\"prop5[0]\":\"2017-09-03\",\"prop5[1]\":\"2017-09-04\",\"prop5[2]\":\"2017-09-05\",\"prop6[0]\":\"2017-09-08T19:04:14.480Z\",\"prop6[1]\":\"2017-09-03T19:04:14.480Z\"}";
            runTest(test, expectedResult);
        }

        [Fact]
        public void Test_json_different_type_properties_flat_successfull()
        {
            var test = "{\"prop1\":{\"prop2\":\"textprop\"},\"prop3\":{\"prop4\":\"2017-09-08\"},\"prop5\":[\"2017-09-05\"],\"prop6\":{\"prop7\":3}}";
            var expectedResult = "{\"prop1.prop2\":\"textprop\",\"prop3.prop4\":\"2017-09-08\",\"prop5[0]\":\"2017-09-05\",\"prop6.prop7\":3}";
            runTest(test, expectedResult);
        }

        private void runTest(string testJson, string expectedJson)
        {
            var flattener = new Flatter();
            var testObj = JObject.Parse(testJson);
            _output.WriteLine("TEST DATA: " + testObj);

            var result = flattener.Flat(testObj);
            _output.WriteLine("RESULT: " + result);

            var expectedResult = JObject.Parse(expectedJson);
            _output.WriteLine("EXPECTED RESULT: " + expectedResult);
            JToken.DeepEquals(result, expectedResult).Should().BeTrue();
        }
    }
}