# Contributing

We love contributions. To get started contributing you might need:

- [Get started with git](http://rogerdudler.github.io/git-guide)
- [How to create a pull request](https://help.github.com/articles/using-pull-requests)
- [An issue to work on](https://github.com/fluentassertions/FluentAssertions.Autofac/labels/up-for-grabs) - We are
  on [Up for grabs](http://up-for-grabs.net/), our up for grabs issues are tagged `up-for-grabs`
- An understanding of how [we write tests](#writing-tests)

Once you know how to create a pull request and have an issue to work on, just post a comment saying you will work on it.
If you end up not being able to complete the task, please post another comment so others can pick it up.

Issues are also welcome, [failing tests](#writing-tests) are even more welcome.

## Contribution Guidelines

- Try to use feature branches rather than developing on master
- Please include tests covering the change
- The docs are now stored in the repository under the `Docs` folder, please include documentation updates with your PR

## Writing Tests

It is easy to write tests in `FluentAssertions.Autofac`. Test fixtures are located directly beside the code in classes
ending with `_Should.cs`.

### 1. Find appropriate fixture

Find where your issue would logically sit, i.e. find the class closest to your issue.

### 2. Create a test method

We are currently using xUnit, so just create a descriptive test method and attribute it with `[Fact]`.

### 3. Submit a pull request with the failing test

Even better include the fix, but a failing test is a great start .
