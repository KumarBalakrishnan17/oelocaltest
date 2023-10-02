import React, { useEffect, useState } from "react";
import ReactSelect from "react-select";
import {getAssignedUsers, assignUser, unAssignUser } from "../../../api/api";

const PlanProcedureItem = ({ procedure, users,planId }) => {
    const [selectedUsers, setSelectedUsers] = useState([]);

    //call and get assigned user id and map with react select
    useEffect(() => {
        (async () => {
            var assignedUser = await getAssignedUsers(planId,procedure.procedureId);
            let userIds=[];
            assignedUser.map((el,index)=>{userIds.push(el.userId); });         
             var filtered= users.filter((el) => userIds.includes(el.value)); 
            setSelectedUsers(filtered);
        })();
      }, [planId],[procedure]);
    

    const handleAssignUserToProcedure = (e) => {

            let stateUserIds=[];
            selectedUsers.map((el,index)=>{stateUserIds.push(el.value); }); 

            let currentSelectedUserIds=[];
            e.map((el,index)=>{currentSelectedUserIds.push(el.value); });

             var assignUserIds= e.filter((el) => !stateUserIds.includes(el.value));        
             var unAssignUserIds= selectedUsers.filter((el) => !currentSelectedUserIds.includes(el.value));
         //map user id's with plan & procedure
           if(assignUserIds.length>0)
           {
                (async () => {
                        await assignUser(planId,procedure.procedureId,assignUserIds[0].value);
                        setSelectedUsers(e);                              
                 })();
           }
           //remove mapping of user id's with plan & procedure
           if(unAssignUserIds.length>0)
           {
            unAssignUserIds.map((el,index)=>{      
                (async () => {
                    await unAssignUser(planId,procedure.procedureId,el.value);                                     
                    })();                   
            });
            setSelectedUsers(e);          
           }
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
