export interface RunResult {
    run_id: string,
    outcome: number,
    cmpinfo: string,
    stdout: string,
    stderr: string
}

export const EmptyRunResult = {
    run_id: '',
    outcome: 0,
    cmpinfo: '',
    stdout: '',
    stderr: ''
}

export function getResultOutcome(outcome: number) {
    switch (outcome) {
        case 11: return 'Compilation error. The cmpinfo field should offer further explanation'
        case 12: return 'Runtime error. The job compiled but threw an exception at run time that isnt covered by any of the more-specific errors below.'
        case 13: return 'Time limit exceeded. The job was killed before it ran to completion as a result of the  server-specified time limit (or a possible time limit specified via the parameters field of the job request) being reached.'
        case 15: return 'OK. The run ran to completion without any exceptions.'
        case 17: return 'Memory limit exceeded. The job was killed before it ran to completion as a result of the  server-specified maximum memory limit (or a possible memory parameters field of the job request) being reached.'
        case 19: return 'Illegal system call. The task attempted a system call not allowed by this particular server.'
        case 20: return 'Internal error. Something went wrong in the server. Please report this to an administrator.'
        case 21: return 'Server overload. No free Jobe user accounts. Probably something has gone wrong.'
        default: return 'Unknown or pending outcome'
    }
}

