# Migration Guide to v2.11
Version 2.11 comes with some breaking-changes. You should follow this docuemnt to migrate your code to the new version properly.

You can see related PRs with breaking changes [from here](https://github.com/enisn/UraniumUI/pulls?q=is%3Aopen+is%3Apr+milestone%3Av2.11+label%3A%22breaking-change+%F0%9F%92%94%22)


## Changes

- `ValidationBinding` is obsolete now. You should use `DataAnnotationsBehavior` instead of `v:ValidationBinding`.

    - Replace `v:ValidationBinding` with the regular `Binding` in XAML.
    - Add `DataAnnotationsBehavior` to the `TextField.Behaviors` or any other control that you want to validate.

        - **Before:**
            ```xml
            <material:TextField Text="{v:ValidationBinding Email}"/>
            ```

        - **After:**

            ```xml
            <material:TextField Text="{Binding Email}">
                <material:TextField.Behaviors>
                    <v:DataAnnotationsBehavior Binding="{Binding Email}" />
                </material:TextField.Behaviors>
            </material:TextField>
            ```