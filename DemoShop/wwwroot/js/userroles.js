$('#submit').click(function () {
    tree = $("#rolestreeview").data("ejTreeView");
    checkedNodes = tree.getCheckedNodes();
    var userRoles = [];  
    for (i = 0; i < checkedNodes.length; i++) {
        var treeItem = tree.getTreeData(checkedNodes[i].id);
        var itemName = treeItem[0].Name;
        if (treeItem[0].ParentId !== null) {
            if (itemName !== null) {
                $('#userForm').append('<input type="hidden" name="NewRoles" value="' + itemName + '" />');
            }
        }
    } 
});