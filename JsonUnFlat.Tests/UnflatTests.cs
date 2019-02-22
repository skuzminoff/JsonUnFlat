using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

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

        [Fact]
        public void Test_array_primitive_values_unflattened()
        {
            var testFlatJson = "{\"prop1[0]\":123,\"prop1[1]\":234}";
            var expectedUnflattenedJson = "{\"prop1\":[123, 234]}";
            runTest(testFlatJson, expectedUnflattenedJson);
        }

        [Fact]
        public void Test_array_combine_values_unflattened()
        {
            var testFlatJson = "{\"prop1[0]\":123,\"prop1[1].prop3\":234}";
            var expectedUnflattenedJson = "{\"prop1\":[123, {\"prop3\":234}]}";
            runTest(testFlatJson, expectedUnflattenedJson);
        }

        [Fact]
        public void Test_json_has_datetime_prop_unflat_successfull()
        {
            var testObjString = "{\"prop1.prop2\":\"2017-09-08T19:04:14.480Z\",\"prop3.prop4\":\"2017-09-08\",\"prop5[0]\":\"2017-09-03\",\"prop5[1]\":\"2017-09-04\",\"prop5[2]\":\"2017-09-05\",\"prop6[0]\":\"2017-09-08T19:04:14.480Z\",\"prop6[1]\":\"2017-09-03T19:04:14.480Z\"}";
            var expectedResultString = "{\"prop1\":{\"prop2\":\"2017-09-08T19:04:14.480Z\"},\"prop3\":{\"prop4\":\"2017-09-08\"},\"prop5\":[\"2017-09-03\",\"2017-09-04\",\"2017-09-05\"],\"prop6\":[\"2017-09-08T19:04:14.480Z\",\"2017-09-03T19:04:14.480Z\"]}";
            runTest(testObjString, expectedResultString);
        }

        [Fact]
        public void Test_json_different_type_properties_unflat_successfull()
        {
           var  testObjString = "{\"prop1.prop2\":\"textprop\",\"prop3.prop4\":\"2017-09-08\",\"prop5[0]\":\"2017-09-05\",\"prop6.prop7\":3}";
           var expectedResultString = "{\"prop1\":{\"prop2\":\"textprop\"},\"prop3\":{\"prop4\":\"2017-09-08\"},\"prop5\":[\"2017-09-05\"],\"prop6\":{\"prop7\":3}}";
           runTest(testObjString, expectedResultString);
        }

        private void runTest(string testJson, string expectedResultJson)
        {
            var testObj = JObject.Parse(testJson);
            _output.WriteLine("TEST DATA: " + testObj);
            var unflattener = new Unflatter();

            var result = unflattener.Unflat(testObj);
            _output.WriteLine("RESULT: " + result);
            var expectedResult = JObject.Parse(expectedResultJson);
            _output.WriteLine("EXPECTED RESULT: " + expectedResult);
            JToken.DeepEquals(result, expectedResult).Should().BeTrue();
        }
    }
}