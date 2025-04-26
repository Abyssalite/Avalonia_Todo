function test(numArr: number[]): boolean{
    return numArr.some((num) => num === 6);

}

class Person {
    name: string;
    age: number;
    constructor(name: string, age: number) {
        this.name = name;
        this.age = age;
    }

    greet(): void {
        console.log(`Hi, I'm ${this.name} and I'm ${this.age} years old.`);
    }
}

if (require.main === module) {
    console.log("Running as the main module!");
    main();
}

function main() {    
    let numArr = [1,2,3,4,5];

    console.log("Executing the main script...");
    console.log(test(numArr));

    const person = new Person("John", 30);
    person.greet();

}


