package com.clinic.ms03.appointments;

import org.springframework.stereotype.Component;

@Component
public class RoomAssignmentPolicy {

    public String selectAssignmentStrategy() {
        // Canonical US-014 order:
        // 1) doctor's fixed room
        // 2) same-department least-recently-used room
        // 3) unassigned + alert
        return "FIXED_ROOM_THEN_DEPARTMENT_LRU_THEN_UNASSIGNED";
    }
}
