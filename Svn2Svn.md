# Overview #

Svn2Svn is a tool that copy change sets across SVN repositories. It is also a repository sync tool that requires neither to start at revision zero nor to copy entire repositories from root.

# Feature List #

  * Copies change sets from one SVN repository to another.
  * Supports non-rooted path (subtree) for both source and destination.
  * Doesn't require source and destination to have same path.
  * Copies node properties so that properties like svn:ignore and svn:external are preserved in the destination repository.
  * Copies revision properties so that author and date/time of revisions are preserved in destination repository.(1)
  * Doesn't require to starting from zero revision.
  * Can optionally specify source revision range.
  * Supports move and copy in addition to add/delete/modify.
  * Able to gracefully stop and resume from stop or error.
  * Able to auto-resync and copy over new change sets.(1)
  * Support both command line and Windows UI.

_Note (1) features require destination repository support revision property editing._

# Installation #

Download the zip file from the download area and unzip to any directory you want. Then just run the executable.
  * `Svn2Svn.exe` - Windows user interface
  * `Svn2SvnConsole.exe` - Command line utility.

Download [latest release](http://code.google.com/p/kennethxublogsource/downloads/list?can=2&q=svn2svn+featured&colspec=Filename+Summary+Uploaded+ReleaseDate+Size+DownloadCount)

# Source code #
http://kennethxublogsource.googlecode.com/svn/trunk/Svn2Svn

# User Guide #
http://kennethxu.blogspot.com/2012/01/svn2svn-copy-and-sync-between-svn.html