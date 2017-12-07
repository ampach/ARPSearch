# ARPSearch

# Description

**ARPSearch** is a module with main idea to save developers' time by providing a base content search interface for fast use and easy customization. Currently, the module represents a wrapper for *Sitecore Content Search*, works with any search providers like *Lucene*, *SOLR*, *COVEO* and etc. The wrapper has a default rendering and allows to manage search configuration in the Sitecore content tree.

# Features

- Predefined Search Result Page.
- Multiple search configurations in the Sitecore tree.
- Multiple roots setting for search.
- Filtering results by template.
- Pagination support.
- Query String based filtering.
- Flexible facet configuration directly in Sitecore.
- Deep customization readiness.

# Quick Start

The ARPSearch module has a search result rendering out of the box. After module installation (please see the “[How to install](https://github.com/ampach/ARPSearch#how-to-install)”  section), you can start using search on your site in a few clicks. Currently, the installation package provides a sample search configuration, which serves to help you with further search configuration. You just need to create a search page (please see the “[How to create search page](https://github.com/ampach/ARPSearch#how-to-create-search-page)” section) and publish your website.

## How to install

Follow the steps below for installing module:

- Make sure that you have installed Sitecore v8.0 or newer;
- Install downloaded package using installation wizard;
- In case of having any conflicts during the installation, choose the "Skip" option to resolve it.

## How to uninstall

If you don’t like the module or in some other reasons you want to uninstall it, remove files by the following paths:

- Views\ARPSearch
- App_Config\Include\ARPSearch
- ARPSearch
- bin\ARPSearch.dll
- bin\ARPSearch.UI.dll
 
And the following items in the Sitecore tree:

- /sitecore/layout/Layouts/Modules/ARPSearch
- /sitecore/layout/Models/Modules/ARPSearch
- /sitecore/layout/Renderings/Modules/ARPSearch
- /sitecore/system/Settings/Rules/Insert Options/Rules/ARPSearch
- /sitecore/system/Modules/APRSearch
- /sitecore/templates/Branches/Modules/ARPSearch
- /sitecore/templates/Modules/APRSearch

Finally, publish your website.

## How to create search page

# Basic Search Configuration


# Advanced Usage

Let’s imagine that you need more than basic functionality. 
For example, ARPSearch has three type of facets (dropdown, checkbox list and checkbox) out of the box but you need more; or you want to extend the search with some extra filters, orderings and fields. You can also implement your own search page in case ours doesn’t match your general requirements. It is possible and a lot more!
