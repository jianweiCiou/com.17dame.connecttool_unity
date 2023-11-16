---
id: nameofpage
title: Template - Name of page
---

Copy this `markdown.md` file located in [*/reference/templates* folder](https://github.com/Unity-Technologies/com.unity.multiplayer.docs/blob/master/reference/templates/markdown.md) in the repository or cloned local files. In the */versioned_docs* folder, paste it in a version folder and location. This file has the minimum code needed to write content. For extensive Markdown features, see [Markdown Guide and Code](../template.md).

Write a quick introduction. Answer the following questions in it: what is this (feature, code example), what does it solve, what is the result?

## Requirements

Enable deep linking for Android applications



## Section of content

Write about the feature, task, or anything else that may be helpful using an H2. 

You can add code samples:

```markdown title="Code Example"
<intent-filter>
  <action android:name="android.intent.action.VIEW" />
  <category android:name="android.intent.category.DEFAULT" />
  <category android:name="android.intent.category.BROWSABLE" />
  <data android:scheme="unitydl" android:host="mylink" />
</intent-filter>
```

### Sub section

If you need to add smaller sections or break up instructions, use an H3.

To write instructions, add a numbered list using 1. for each line:

1. Open the project in Unity.
1. Click **Menu** > **Item** > **Option** and select a *Feature*.

To add an image, save the *.png* or *.jpg* to the */static/img* folder. For example:

![Image alt text](/img/example-img.png)