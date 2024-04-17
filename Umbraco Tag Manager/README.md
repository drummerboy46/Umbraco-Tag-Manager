# Our.Umbraco.Community.TagManager

Useful Tag utilities for Umbraco 10, 12 & 13.

Initially based on code from https://github.com/usome/UmbracoTagManager and https://github.com/huwred/Tag-Lister completely re-written using the repository pattern and improved UI.

This package installs a custom section within the administration area and a new DataType.

## Tag Manager
Creates a tree view of all Tags that have been created by the Umbraco Tag Datatype or assigned by my Tag List Property Editor. 

The tree is split into separate branches for each tag Group created - useful if you run multiple blogs on your site, or have multiple Tag Groups defined in a site.

1. Create Tag Groups.
1. Create and change Tags.
2. Delete Tags, single & multiple.
3. Indication of how many times the Tag is currently being used (number in brackets in Group overview).
4. Ability to merge Tags - useful in cases of spelling mistakes, cleaning up Tags, etc.
5. Links to content and media where Tag is used.
6. Language variant/invariant support.

## Tag List Property Editor
Provides a multi select (read-only) list of all tags assigned to the group. 

1. Choose one or more tags from the list
2. Works on Content and Media Doctypes.
3. Language variant/invariant support (Media is invariant only)
3. Tags are saved to the default internal and external indexes in CSV format.

## Roadmap
2. Content editors can add tags to a Tag List, enabled in a Pre-value switch.
3. Umbraco 14 support