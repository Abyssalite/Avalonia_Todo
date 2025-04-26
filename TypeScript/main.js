function test(numArr) {
    return numArr.some(function (num) { return num === 6; });
}
var Person = /** @class */ (function () {
    function Person(name, age) {
        this.name = name;
        this.age = age;
    }
    Person.prototype.greet = function () {
        console.log("Hi, I'm ".concat(this.name, " and I'm ").concat(this.age, " years old."));
    };
    return Person;
}());
if (require.main === module) {
    console.log("Running as the main module!");
    main();
}
function main() {
    var numArr = [1, 2, 3, 4, 5];
    console.log("Executing the main script...");
    console.log(test(numArr));
    var person = new Person("John", 30);
    person.greet();
}
