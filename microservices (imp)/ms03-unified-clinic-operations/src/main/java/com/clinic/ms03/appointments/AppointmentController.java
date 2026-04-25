package com.clinic.ms03.appointments;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/appointments")
public class AppointmentController {

    private final RoomAssignmentPolicy roomAssignmentPolicy;

    public AppointmentController(RoomAssignmentPolicy roomAssignmentPolicy) {
        this.roomAssignmentPolicy = roomAssignmentPolicy;
    }

    @GetMapping("/status")
    public ResponseEntity<String> status() {
        // US-011..US-015 appointment domain placeholder.
        return ResponseEntity.ok("Appointment module scaffold: " + roomAssignmentPolicy.selectAssignmentStrategy());
    }
}
