Guidelines for contributing changes
-----------------------------------

For some background information regarding the branching model used by this project, refer to the article [A successful Git branching model][1]. The ideas from that article have been combined with the collaborative development techniques recommended by GitHub (specifically [Using Pull Requests][2]) to form these procedures.

### Pre-requisities ###

* [Git][3] installed.
* A strong understanding of Git. For an introduction, read [Getting Started - Git Basics][4].
* A GitHub account and Pull access to the [NSIP][5] repositories.

### Defined Repositories ###

The procedures following refer to 3 Git repositories:

* The remote (_upstream_) [Sif3Framework-dotNet][6] repository containing a _central_ copy of the code.
* Your remote repository created within your GitHub account.
* Your local repository residing on your local machine.

### Getting the code ###

1. Fork the [Sif3Framework-dotNet][6] repository into your remote repository (click on the "Fork" button).
2. Clone your fork to create your local repository. This will create a _Sif3Framework-dotNet_ directory on your local machine.

    ```dos
    c:\dev> git clone https://github.com/USERNAME/Sif3Framework-dotNet.git
    ```

3. Add the _upstream_ ([Sif3Framework-dotNet][6]) repository as a remote. This will allow you to keep track of the [Sif3Framework-dotNet][6] repository and pull in updates.

    ```dos
    c:\dev> cd Sif3Framework-dotNet
    c:\dev\Sif3Framework-dotNet> git remote add upstream https://github.com/nsip/Sif3Framework-dotNet.git
    ```

4. In your local repository, set up a tracking branch for the _develop_ branch of your remote repository and switch to it.

    ```dos
    c:\dev\Sif3Framework-dotNet> git checkout -t origin/develop
    ```

5. If you wish to create a _topic_ branch in your local repository, please ensure that it is based upon the _develop_ branch. __Do not base your _topic_ branch on the _master_ branch!__

    ```dos
    c:\dev\Sif3Framework-dotNet> git branch ISSUE_XXX develop
    c:\dev\Sif3Framework-dotNet> git checkout ISSUE_XXX
    ```

### Ensuring you remote repository is up-to-date ###

To sync your remote repository with the _upstream_ ([Sif3Framework-dotNet][6]) repository, fetch all _upstream_ changes into your local repository and then push them from your local repository to your remote repository.

1. Fetch all changes from the _upstream_ repository and merge into your local repository.

    ```dos
    c:\dev\Sif3Framework-dotNet> git fetch upstream
    c:\dev\Sif3Framework-dotNet> git checkout develop
    c:\dev\Sif3Framework-dotNet> git merge upstream/develop
    ```

2. Push the updates in your local repository to your remote repository.

    ```dos
    c:\dev\Sif3Framework-dotNet> git push origin develop
    ```

### Submitting a change ###

Before submitting a change, ensure that your local repository is up-to-date with the _upstream_ repository (as outlined in the previous section).

1. Commit all local repository changes with an informative message. The _-a_ flag skips _staging_.

    ```dos
    c:\dev\Sif3Framework-dotNet> git checkout develop
    c:\dev\Sif3Framework-dotNet> git commit -a -m MESSAGE
    ```

2. Push the updates in your local repository to your remote repository.

    ```dos
    c:\dev\Sif3Framework-dotNet> git push origin develop
    ```

### Issue a Pull Request ###

The [Using Pull Requests][2] article provides a comprehensive guide for issuing a Pull Request.

1. Browse to your remote (forked) repository on the GitHub site.

2. Switch to the appropriate (_develop_) branch and press the "Pull Request" button.

3. Review the Pull Request details, and provide a meaningful title and description of your change. On the "Comparing changes" page, it is important to ensure that the _base branch_ (base:) is set to _develop_ and the _head branch_ (compare:) is set to your _develop_ branch. Pull Requests will not be accepted against the _master_ branch of the [Sif3Framework-dotNet][6] repository.

4. Press the "Send pull request" button.

[1]: http://nvie.com/posts/a-successful-git-branching-model
[2]: https://help.github.com/articles/using-pull-requests
[3]: http://git-scm.com/downloads
[4]: http://git-scm.com/book/en/Getting-Started-Git-Basics
[5]: https://github.com/nsip
[6]: https://github.com/nsip/Sif3Framework-dotNet
